USE [UserReview]
GO

-- ============================================================================
-- vw_UserRoleAssignment
--
-- View over Fact_UserRoleAssignment showing actual values instead of keys:
-- employee name, system username, role code/description, source system, etc.
-- ============================================================================

CREATE OR ALTER VIEW dbo.vw_UserRoleAssignment
AS
SELECT
  ura.AssignmentKey,
  ura.SystemIdentityKey,
  ura.RoleKey,
  -- Identity / user
  si.SystemUsername,
  emp.AD_Username       AS EmployeeADUsername,
  emp.FullName          AS EmployeeFullName,
  emp.Email             AS EmployeeEmail,
  emp.Country           AS EmployeeCountry,
  si.Plant,
  si.UserType           AS EmployeeUserType,
  -- Role
  r.RoleCode,
  r.RoleDescription,
  -- Fact columns
  ura.SourceSystem,
  CASE WHEN ura.IsActive = 1 THEN 'Yes' ELSE 'No' END AS IsActive,
  ura.ETL_LoadDate
FROM   Fact_UserRoleAssignment ura
JOIN   Dim_SystemIdentity       si  ON ura.SystemIdentityKey = si.SystemIdentityKey
JOIN   Dim_Employee             emp ON si.EmployeeKey        = emp.EmployeeKey
JOIN   Dim_Role                 r   ON ura.RoleKey           = r.RoleKey;
GO
