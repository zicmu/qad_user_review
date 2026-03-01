USE [UserReview]
GO

-- ============================================================================
-- sp_IngestSourceData
--
-- Master ETL procedure.  Assumes the Stg_Raw_* tables have already been
-- populated from the source files (by the C# application or BULK INSERT).
--
-- Pipeline:
--   Phase 1  – Raw staging  →  Canonical staging (Stg_QAD/SAP_Users/Roles)
--   Phase 2  – Canonical staging  →  Dim_Employee
--   Phase 3  – Canonical staging  →  Dim_SystemIdentity
--   Phase 4  – Canonical staging  →  Dim_Role
--   Phase 5  – Canonical staging  →  Bridge_OrgChart
--   Phase 6  – Canonical staging  →  Fact_UserRoleAssignment
-- ============================================================================

CREATE OR ALTER PROCEDURE dbo.sp_IngestSourceData
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @Now DATETIME2 = GETUTCDATE();
    DECLARE @Today DATE     = CAST(@Now AS DATE);

    BEGIN TRY
        BEGIN TRANSACTION;

        -- ================================================================
        -- PHASE 1 : Raw  →  Canonical Staging
        -- ================================================================

        TRUNCATE TABLE Stg_QAD_Users;
        TRUNCATE TABLE Stg_QAD_Roles;
        TRUNCATE TABLE Stg_SAP_Users;
        TRUNCATE TABLE Stg_SAP_Roles;

        -- ---- 1A  QAD Users ----
        -- QAD2007 active users enriched with org-hier data
        -- Usernames normalized to A-Za-z0-9 (accents e.g. ï -> i) via fn_NormalizeUsername
        INSERT INTO Stg_QAD_Users
               (SystemUsername, FullName, Plant, Email, Country,
                ManagerUsername, UserType, Active, SourceSystem, ETL_FileName)
        SELECT  LOWER(dbo.fn_NormalizeUsername(u.UserID)),
                COALESCE(o.Employee, u.UserName),
                o.Plant,
                o.Email,
                o.Country,
                LOWER(dbo.fn_NormalizeUsername(o.ManagerUserName)),
                COALESCE(o.Valid, 'User'),
                'Yes',
                'QAD2007',
                'Users2007 + OrgHier'
        FROM   Stg_Raw_QAD_Users2007 u
               LEFT JOIN Stg_Raw_QAD_OrgHier o
                   ON LOWER(dbo.fn_NormalizeUsername(u.UserID)) = LOWER(dbo.fn_NormalizeUsername(o.Username));

        -- QAD2008 active users enriched with org-hier data
        INSERT INTO Stg_QAD_Users
               (SystemUsername, FullName, Plant, Email, Country,
                ManagerUsername, UserType, Active, SourceSystem, ETL_FileName)
        SELECT  LOWER(dbo.fn_NormalizeUsername(u.UserID)),
                COALESCE(o.Employee, u.UserName),
                o.Plant,
                o.Email,
                o.Country,
                LOWER(dbo.fn_NormalizeUsername(o.ManagerUserName)),
                COALESCE(o.Valid, 'User'),
                'Yes',
                'QAD2008',
                'Users2008 + OrgHier'
        FROM   Stg_Raw_QAD_Users2008 u
               LEFT JOIN Stg_Raw_QAD_OrgHier o
                   ON LOWER(dbo.fn_NormalizeUsername(u.UserID)) = LOWER(dbo.fn_NormalizeUsername(o.Username));

        -- ---- 1B  QAD Roles (only active users) ----
        -- QAD2007 roles  (DEFRGB ∩ Users2007)
        INSERT INTO Stg_QAD_Roles
               (SystemUsername, RoleCode, RoleDescription, SourceSystem, ETL_FileName)
        SELECT  DISTINCT
                LOWER(dbo.fn_NormalizeUsername(r.UserID)),
                r.GroupCode,
                r.GroupDescription,
                'QAD2007',
                'DEFRGB'
        FROM   Stg_Raw_QAD_DEFRGB r
               INNER JOIN Stg_Raw_QAD_Users2007 u
                   ON LOWER(dbo.fn_NormalizeUsername(r.UserID)) = LOWER(dbo.fn_NormalizeUsername(u.UserID));

        -- QAD2008 roles  (ITRS ∩ Users2008)
        INSERT INTO Stg_QAD_Roles
               (SystemUsername, RoleCode, RoleDescription, SourceSystem, ETL_FileName)
        SELECT  DISTINCT
                LOWER(dbo.fn_NormalizeUsername(r.UserID)),
                r.GroupCode,
                r.GroupDescription,
                'QAD2008',
                'ITRS'
        FROM   Stg_Raw_QAD_ITRS r
               INNER JOIN Stg_Raw_QAD_Users2008 u
                   ON LOWER(dbo.fn_NormalizeUsername(r.UserID)) = LOWER(dbo.fn_NormalizeUsername(u.UserID));

        -- ---- 1C  SAP Users ----
        -- Every user in the SAP org-hier is considered active
        INSERT INTO Stg_SAP_Users
               (SystemUsername, FullName, Plant, Email, Country,
                ManagerUsername, UserType, Active, SourceSystem, ETL_FileName)
        SELECT  LOWER(dbo.fn_NormalizeUsername(o.UserId)),
                o.FullName,
                NULL,
                NULL,
                NULL,
                LOWER(dbo.fn_NormalizeUsername(o.ManagerUserId)),
                'Regular',
                'Yes',
                'SAP',
                'SAP OrgHier'
        FROM   Stg_Raw_SAP_OrgHier o;

        -- ---- 1D  SAP Roles ----
        -- Only roles for users present in the org-hier (active)
        INSERT INTO Stg_SAP_Roles
               (SystemUsername, RoleCode, RoleDescription, SourceSystem, ETL_FileName)
        SELECT  DISTINCT
                LOWER(dbo.fn_NormalizeUsername(r.Benutzer)),
                r.Rolle,
                r.RolleBeschreibung,
                'SAP',
                'SAP Rollen'
        FROM   Stg_Raw_SAP_Rollen r
               INNER JOIN Stg_Raw_SAP_OrgHier o
                   ON LOWER(dbo.fn_NormalizeUsername(r.Benutzer)) = LOWER(dbo.fn_NormalizeUsername(o.UserId));


        -- ================================================================
        -- PHASE 2 : Canonical Staging  →  Dim_Employee
        -- ================================================================
        -- Build a unified, deduplicated employee list.
        -- Priority: QAD org-hier data is richest, then QAD user files,
        -- then SAP org-hier for employees not seen in QAD.

        ;WITH UnifiedEmployees AS (
            -- All employees from QAD org-hier (most complete)
            SELECT  LOWER(dbo.fn_NormalizeUsername(Username)) AS AD_Username,
                    Employee        AS FullName,
                    Email,
                    Country,
                    1               AS Priority
            FROM   Stg_Raw_QAD_OrgHier
            WHERE  Username IS NOT NULL AND LTRIM(RTRIM(Username)) <> ''

            UNION ALL

            -- Managers from QAD org-hier who may not appear as employees
            SELECT  LOWER(dbo.fn_NormalizeUsername(ManagerUserName)),
                    Manager,
                    NULL,
                    NULL,
                    2
            FROM   Stg_Raw_QAD_OrgHier
            WHERE  ManagerUserName IS NOT NULL AND LTRIM(RTRIM(ManagerUserName)) <> ''
              AND  NOT EXISTS (
                       SELECT 1 FROM Stg_Raw_QAD_OrgHier x
                       WHERE  LOWER(dbo.fn_NormalizeUsername(x.Username)) = LOWER(dbo.fn_NormalizeUsername(Stg_Raw_QAD_OrgHier.ManagerUserName))
                   )

            UNION ALL

            -- QAD2007 users missing from org-hier
            SELECT  LOWER(dbo.fn_NormalizeUsername(UserID)), UserName, NULL, NULL, 3
            FROM   Stg_Raw_QAD_Users2007
            WHERE  NOT EXISTS (
                       SELECT 1 FROM Stg_Raw_QAD_OrgHier o
                       WHERE  LOWER(dbo.fn_NormalizeUsername(o.Username)) = LOWER(dbo.fn_NormalizeUsername(Stg_Raw_QAD_Users2007.UserID))
                   )

            UNION ALL

            -- QAD2008 users missing from org-hier
            SELECT  LOWER(dbo.fn_NormalizeUsername(UserID)), UserName, NULL, NULL, 4
            FROM   Stg_Raw_QAD_Users2008
            WHERE  NOT EXISTS (
                       SELECT 1 FROM Stg_Raw_QAD_OrgHier o
                       WHERE  LOWER(dbo.fn_NormalizeUsername(o.Username)) = LOWER(dbo.fn_NormalizeUsername(Stg_Raw_QAD_Users2008.UserID))
                   )

            UNION ALL

            -- SAP users: use SAP UserId as AD_Username when no QAD match
            SELECT  LOWER(dbo.fn_NormalizeUsername(s.UserId)),
                    s.FullName,
                    NULL,
                    NULL,
                    5
            FROM   Stg_Raw_SAP_OrgHier s
            WHERE  NOT EXISTS (
                       SELECT 1 FROM Stg_Raw_QAD_OrgHier o
                       WHERE  LOWER(o.Employee) = LOWER(s.FullName)
                   )

            UNION ALL

            -- SAP managers who are not SAP employees themselves
            SELECT  LOWER(dbo.fn_NormalizeUsername(s.ManagerUserId)),
                    s.ManagerName,
                    s.ManagerEmail,
                    NULL,
                    6
            FROM   Stg_Raw_SAP_OrgHier s
            WHERE  s.ManagerUserId IS NOT NULL AND LTRIM(RTRIM(s.ManagerUserId)) <> ''
              AND  NOT EXISTS (
                       SELECT 1 FROM Stg_Raw_SAP_OrgHier x
                       WHERE  LOWER(dbo.fn_NormalizeUsername(x.UserId)) = LOWER(dbo.fn_NormalizeUsername(s.ManagerUserId))
                   )
              AND  NOT EXISTS (
                       SELECT 1 FROM Stg_Raw_QAD_OrgHier o
                       WHERE  LOWER(o.Employee) = LOWER(s.ManagerName)
                   )
        ),
        Ranked AS (
            SELECT  AD_Username,
                    FullName,
                    Email,
                    Country,
                    ROW_NUMBER() OVER (PARTITION BY AD_Username ORDER BY Priority) AS rn
            FROM   UnifiedEmployees
        )
        MERGE Dim_Employee AS tgt
        USING (SELECT AD_Username, FullName, Email, Country FROM Ranked WHERE rn = 1) AS src
            ON LOWER(tgt.AD_Username) = src.AD_Username
        WHEN MATCHED THEN
            UPDATE SET FullName  = src.FullName,
                       Email     = COALESCE(src.Email, tgt.Email),
                       Country   = COALESCE(src.Country, tgt.Country),
                       IsActive  = 1,
                       UpdatedAt = @Now
        WHEN NOT MATCHED THEN
            INSERT (AD_Username, FullName, Email, Country, IsActive, CreatedAt, UpdatedAt)
            VALUES (src.AD_Username, src.FullName, src.Email, src.Country, 1, @Now, @Now);


        -- ================================================================
        -- PHASE 3 : Canonical Staging  →  Dim_SystemIdentity
        -- ================================================================
        -- One row per (SystemUsername, SourceSystem) combination.

        ;WITH AllIdentities AS (
            -- QAD identities
            SELECT  s.SystemUsername,
                    s.SourceSystem,
                    s.Plant,
                    s.UserType,
                    LOWER(s.SystemUsername) AS AD_Username_Match
            FROM   Stg_QAD_Users s

            UNION ALL

            -- SAP identities: try matching to QAD employee by name
            SELECT  s.SystemUsername,
                    s.SourceSystem,
                    s.Plant,
                    s.UserType,
                    COALESCE(
                        (SELECT TOP 1 LOWER(dbo.fn_NormalizeUsername(o.Username))
                         FROM   Stg_Raw_QAD_OrgHier o
                         WHERE  LOWER(o.Employee) = LOWER(s.FullName)),
                        LOWER(s.SystemUsername)
                    ) AS AD_Username_Match
            FROM   Stg_SAP_Users s
        )
        MERGE Dim_SystemIdentity AS tgt
        USING (
            SELECT DISTINCT  ai.SystemUsername,
                    ai.SourceSystem,
                    ai.Plant,
                    ai.UserType,
                    e.EmployeeKey
            FROM   AllIdentities ai
                   INNER JOIN Dim_Employee e
                       ON LOWER(e.AD_Username) = ai.AD_Username_Match
        ) AS src
            ON  tgt.SystemUsername = src.SystemUsername
            AND tgt.SourceSystem  = src.SourceSystem
        WHEN MATCHED THEN
            UPDATE SET Plant        = COALESCE(src.Plant, tgt.Plant),
                       UserType     = COALESCE(src.UserType, tgt.UserType),
                       EmployeeKey  = src.EmployeeKey,
                       IsActive     = 1,
                       ETL_LoadDate = @Now
        WHEN NOT MATCHED THEN
            INSERT (EmployeeKey, SystemUsername, SourceSystem, Plant, UserType, IsActive, ETL_LoadDate)
            VALUES (src.EmployeeKey, src.SystemUsername, src.SourceSystem, src.Plant, src.UserType, 1, @Now);


        -- ================================================================
        -- PHASE 4 : Canonical Staging  →  Dim_Role
        -- ================================================================

        ;WITH AllRoles AS (
            SELECT RoleCode, RoleDescription, SourceSystem,
                   ROW_NUMBER() OVER (PARTITION BY RoleCode, SourceSystem
                                      ORDER BY RoleDescription) AS rn
            FROM (
                SELECT RoleCode, RoleDescription, SourceSystem FROM Stg_QAD_Roles
                UNION ALL
                SELECT RoleCode, RoleDescription, SourceSystem FROM Stg_SAP_Roles
            ) combined
        )
        MERGE Dim_Role AS tgt
        USING (SELECT RoleCode, RoleDescription, SourceSystem FROM AllRoles WHERE rn = 1) AS src
            ON  tgt.RoleCode     = src.RoleCode
            AND tgt.SourceSystem = src.SourceSystem
        WHEN MATCHED THEN
            UPDATE SET RoleDescription = COALESCE(src.RoleDescription, tgt.RoleDescription),
                       IsActive        = 1
        WHEN NOT MATCHED THEN
            INSERT (RoleCode, RoleDescription, SourceSystem, IsActive)
            VALUES (src.RoleCode, src.RoleDescription, src.SourceSystem, 1);


        -- ================================================================
        -- PHASE 5 : Canonical Staging  →  Bridge_OrgChart
        -- ================================================================
        -- QAD: org hier applies to both QAD2007 & QAD2008
        -- SAP: org hier from SAP-specific file

        ;WITH OrgLinks AS (
            -- QAD2007 links
            SELECT  emp.EmployeeKey,
                    mgr.EmployeeKey AS ReviewerKey,
                    'QAD2007'       AS SourceSystem
            FROM   Stg_QAD_Users s
                   INNER JOIN Dim_Employee emp ON LOWER(emp.AD_Username) = s.SystemUsername
                   INNER JOIN Dim_Employee mgr ON LOWER(mgr.AD_Username) = s.ManagerUsername
            WHERE  s.SourceSystem    = 'QAD2007'
              AND  s.ManagerUsername IS NOT NULL
              AND  LTRIM(RTRIM(s.ManagerUsername)) <> ''

            UNION

            -- QAD2008 links
            SELECT  emp.EmployeeKey,
                    mgr.EmployeeKey,
                    'QAD2008'
            FROM   Stg_QAD_Users s
                   INNER JOIN Dim_Employee emp ON LOWER(emp.AD_Username) = s.SystemUsername
                   INNER JOIN Dim_Employee mgr ON LOWER(mgr.AD_Username) = s.ManagerUsername
            WHERE  s.SourceSystem    = 'QAD2008'
              AND  s.ManagerUsername IS NOT NULL
              AND  LTRIM(RTRIM(s.ManagerUsername)) <> ''

            UNION

            -- SAP links: resolve manager via prioritised strategies
            SELECT  DISTINCT
                    si.EmployeeKey,
                    mgr.ReviewerKey,
                    'SAP'
            FROM   Stg_SAP_Users s
                   INNER JOIN Dim_SystemIdentity si
                       ON UPPER(si.SystemUsername) = UPPER(s.SystemUsername)
                      AND si.SourceSystem = 'SAP'
                   CROSS APPLY (
                       SELECT TOP 1 ReviewerKey
                       FROM (
                           -- Strategy 1: ManagerUserId matches an AD_Username
                           SELECT e.EmployeeKey AS ReviewerKey, 1 AS Priority
                           FROM   Dim_Employee e
                           WHERE  LOWER(e.AD_Username) = LOWER(s.ManagerUsername)
                           UNION ALL
                           -- Strategy 2: manager's full name matches
                           SELECT e.EmployeeKey, 2
                           FROM   Dim_Employee e
                                  INNER JOIN Stg_Raw_SAP_OrgHier raw
                                      ON raw.UserId = s.SystemUsername
                           WHERE  LOWER(e.FullName) = LOWER(raw.ManagerName)
                           UNION ALL
                           -- Strategy 3: manager's email matches
                           SELECT e.EmployeeKey, 3
                           FROM   Dim_Employee e
                                  INNER JOIN Stg_Raw_SAP_OrgHier raw
                                      ON raw.UserId = s.SystemUsername
                           WHERE  e.Email IS NOT NULL
                             AND  LOWER(e.Email) = LOWER(raw.ManagerEmail)
                       ) candidates
                       ORDER BY Priority
                   ) mgr
            WHERE  s.ManagerUsername IS NOT NULL
              AND  LTRIM(RTRIM(s.ManagerUsername)) <> ''
        )
        MERGE Bridge_OrgChart AS tgt
        USING OrgLinks AS src
            ON  tgt.EmployeeKey  = src.EmployeeKey
            AND tgt.ReviewerKey  = src.ReviewerKey
            AND tgt.SourceSystem = src.SourceSystem
        WHEN MATCHED THEN
            UPDATE SET IsActive     = 1,
                       ETL_LoadDate = @Now
        WHEN NOT MATCHED THEN
            INSERT (EmployeeKey, ReviewerKey, SourceSystem, ValidFrom, ValidTo, IsActive, ETL_LoadDate)
            VALUES (src.EmployeeKey, src.ReviewerKey, src.SourceSystem, @Today, NULL, 1, @Now);

        -- Deactivate org-chart links that no longer appear in source data
        -- (only touch links whose SourceSystem is represented in staging)
        UPDATE oc
        SET    oc.IsActive  = 0,
               oc.ValidTo   = @Today
        FROM   Bridge_OrgChart oc
        WHERE  oc.IsActive = 1
          AND  oc.SourceSystem IN ('QAD2007','QAD2008','SAP')
          AND  NOT EXISTS (
                   SELECT 1
                   FROM   Stg_QAD_Users s
                          INNER JOIN Dim_Employee emp ON LOWER(emp.AD_Username) = s.SystemUsername
                          INNER JOIN Dim_Employee mgr ON LOWER(mgr.AD_Username) = s.ManagerUsername
                   WHERE  s.ManagerUsername IS NOT NULL AND LTRIM(RTRIM(s.ManagerUsername)) <> ''
                     AND  emp.EmployeeKey = oc.EmployeeKey
                     AND  mgr.EmployeeKey = oc.ReviewerKey
                     AND  s.SourceSystem  = oc.SourceSystem
               )
          AND  NOT EXISTS (
                   SELECT 1
                   FROM   Stg_SAP_Users s2
                          INNER JOIN Dim_SystemIdentity si2
                              ON UPPER(si2.SystemUsername) = UPPER(s2.SystemUsername) AND si2.SourceSystem = 'SAP'
                          CROSS APPLY (
                              SELECT TOP 1 candidates.EmployeeKey AS ReviewerKey
                              FROM (
                                  SELECT e.EmployeeKey, 1 AS P FROM Dim_Employee e WHERE LOWER(e.AD_Username) = LOWER(s2.ManagerUsername)
                                  UNION ALL
                                  SELECT e.EmployeeKey, 2   FROM Dim_Employee e INNER JOIN Stg_Raw_SAP_OrgHier r ON r.UserId = s2.SystemUsername WHERE LOWER(e.FullName) = LOWER(r.ManagerName)
                                  UNION ALL
                                  SELECT e.EmployeeKey, 3   FROM Dim_Employee e INNER JOIN Stg_Raw_SAP_OrgHier r ON r.UserId = s2.SystemUsername WHERE e.Email IS NOT NULL AND LOWER(e.Email) = LOWER(r.ManagerEmail)
                              ) candidates(EmployeeKey, P) ORDER BY P
                          ) mgr2
                   WHERE  s2.ManagerUsername IS NOT NULL AND LTRIM(RTRIM(s2.ManagerUsername)) <> ''
                     AND  si2.EmployeeKey   = oc.EmployeeKey
                     AND  mgr2.ReviewerKey  = oc.ReviewerKey
                     AND  oc.SourceSystem   = 'SAP'
               );


        -- ================================================================
        -- PHASE 6 : Canonical Staging  →  Fact_UserRoleAssignment
        -- ================================================================

        ;WITH AllAssignments AS (
            -- QAD
            SELECT  si.SystemIdentityKey,
                    r.RoleKey,
                    sr.SourceSystem
            FROM   Stg_QAD_Roles sr
                   INNER JOIN Dim_SystemIdentity si
                       ON  LOWER(si.SystemUsername) = sr.SystemUsername
                       AND si.SourceSystem         = sr.SourceSystem
                   INNER JOIN Dim_Role r
                       ON  r.RoleCode     = sr.RoleCode
                       AND r.SourceSystem = sr.SourceSystem

            UNION

            -- SAP
            SELECT  si.SystemIdentityKey,
                    r.RoleKey,
                    sr.SourceSystem
            FROM   Stg_SAP_Roles sr
                   INNER JOIN Dim_SystemIdentity si
                       ON  UPPER(si.SystemUsername) = UPPER(sr.SystemUsername)
                       AND si.SourceSystem         = sr.SourceSystem
                   INNER JOIN Dim_Role r
                       ON  r.RoleCode     = sr.RoleCode
                       AND r.SourceSystem = sr.SourceSystem
        )
        MERGE Fact_UserRoleAssignment AS tgt
        USING AllAssignments AS src
            ON  tgt.SystemIdentityKey = src.SystemIdentityKey
            AND tgt.RoleKey           = src.RoleKey
        WHEN MATCHED THEN
            UPDATE SET IsActive     = 1,
                       SourceSystem = src.SourceSystem,
                       ETL_LoadDate = @Now
        WHEN NOT MATCHED THEN
            INSERT (SystemIdentityKey, RoleKey, SourceSystem, IsActive, ETL_LoadDate)
            VALUES (src.SystemIdentityKey, src.RoleKey, src.SourceSystem, 1, @Now);

        -- Deactivate assignments no longer in source data
        UPDATE Fact_UserRoleAssignment
        SET    IsActive = 0, ETL_LoadDate = @Now
        WHERE  IsActive = 1
          AND  NOT EXISTS (
                   SELECT 1
                   FROM   (
                       SELECT si.SystemIdentityKey, r.RoleKey
                       FROM   Stg_QAD_Roles sr
                              INNER JOIN Dim_SystemIdentity si
                                  ON LOWER(si.SystemUsername) = sr.SystemUsername AND si.SourceSystem = sr.SourceSystem
                              INNER JOIN Dim_Role r
                                  ON r.RoleCode = sr.RoleCode AND r.SourceSystem = sr.SourceSystem
                       UNION
                       SELECT si.SystemIdentityKey, r.RoleKey
                       FROM   Stg_SAP_Roles sr
                              INNER JOIN Dim_SystemIdentity si
                                  ON UPPER(si.SystemUsername) = UPPER(sr.SystemUsername) AND si.SourceSystem = sr.SourceSystem
                              INNER JOIN Dim_Role r
                                  ON r.RoleCode = sr.RoleCode AND r.SourceSystem = sr.SourceSystem
                   ) cur
                   WHERE cur.SystemIdentityKey = Fact_UserRoleAssignment.SystemIdentityKey
                     AND cur.RoleKey           = Fact_UserRoleAssignment.RoleKey
               );


        COMMIT TRANSACTION;
        PRINT 'sp_IngestSourceData completed successfully.';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
