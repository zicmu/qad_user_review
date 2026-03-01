USE [UserReview]
GO

-- ============================================================================
-- RAW STAGING TABLES
-- Landing zone for source file imports. Each table mirrors the exact column
-- layout of its corresponding source file (.xlsx). The C# application parses
-- Excel files via ClosedXML and bulk-loads data here; SQL stored procedures
-- then transform onward.
-- ============================================================================

-- ---------- SAP ----------

IF OBJECT_ID('Stg_Raw_SAP_OrgHier', 'U') IS NOT NULL DROP TABLE Stg_Raw_SAP_OrgHier;
GO
CREATE TABLE Stg_Raw_SAP_OrgHier (
    UserId          VARCHAR(50),
    FullName        VARCHAR(100),
    ManagerName     VARCHAR(100),
    ManagerEmail    VARCHAR(100),
    ManagerUserId   VARCHAR(50),
    ETL_LoadDate    DATETIME2    NOT NULL DEFAULT GETUTCDATE(),
    ETL_FileName    VARCHAR(200)
);
GO

IF OBJECT_ID('Stg_Raw_SAP_Rollen', 'U') IS NOT NULL DROP TABLE Stg_Raw_SAP_Rollen;
GO
CREATE TABLE Stg_Raw_SAP_Rollen (
    Benutzer            VARCHAR(50),
    VollstaendigerName  VARCHAR(150),
    Rolle               VARCHAR(200),
    Typ                 VARCHAR(100),
    Zuordnung           VARCHAR(100),
    HROrg               VARCHAR(50),
    RolleBeschreibung   VARCHAR(300),
    Abteilung           VARCHAR(100),
    Startdatum          VARCHAR(50),
    Enddatum            VARCHAR(50),
    Manager             VARCHAR(100),
    ETL_LoadDate        DATETIME2    NOT NULL DEFAULT GETUTCDATE(),
    ETL_FileName        VARCHAR(200)
);
GO

-- ---------- QAD ----------

IF OBJECT_ID('Stg_Raw_QAD_Users2007', 'U') IS NOT NULL DROP TABLE Stg_Raw_QAD_Users2007;
GO
CREATE TABLE Stg_Raw_QAD_Users2007 (
    UserID       VARCHAR(50),
    UserName     VARCHAR(100),
    ETL_LoadDate DATETIME2    NOT NULL DEFAULT GETUTCDATE(),
    ETL_FileName VARCHAR(200)
);
GO

IF OBJECT_ID('Stg_Raw_QAD_Users2008', 'U') IS NOT NULL DROP TABLE Stg_Raw_QAD_Users2008;
GO
CREATE TABLE Stg_Raw_QAD_Users2008 (
    UserID       VARCHAR(50),
    UserName     VARCHAR(100),
    ETL_LoadDate DATETIME2    NOT NULL DEFAULT GETUTCDATE(),
    ETL_FileName VARCHAR(200)
);
GO

IF OBJECT_ID('Stg_Raw_QAD_DEFRGB', 'U') IS NOT NULL DROP TABLE Stg_Raw_QAD_DEFRGB;
GO
CREATE TABLE Stg_Raw_QAD_DEFRGB (
    GroupCode        VARCHAR(50),
    GroupDescription VARCHAR(200),
    Domain           VARCHAR(50),
    UserID           VARCHAR(50),
    UserName         VARCHAR(100),
    ETL_LoadDate     DATETIME2    NOT NULL DEFAULT GETUTCDATE(),
    ETL_FileName     VARCHAR(200)
);
GO

IF OBJECT_ID('Stg_Raw_QAD_ITRS', 'U') IS NOT NULL DROP TABLE Stg_Raw_QAD_ITRS;
GO
CREATE TABLE Stg_Raw_QAD_ITRS (
    GroupCode        VARCHAR(50),
    GroupDescription VARCHAR(200),
    Domain           VARCHAR(50),
    UserID           VARCHAR(50),
    UserName         VARCHAR(100),
    ETL_LoadDate     DATETIME2    NOT NULL DEFAULT GETUTCDATE(),
    ETL_FileName     VARCHAR(200)
);
GO

IF OBJECT_ID('Stg_Raw_QAD_OrgHier', 'U') IS NOT NULL DROP TABLE Stg_Raw_QAD_OrgHier;
GO
CREATE TABLE Stg_Raw_QAD_OrgHier (
    Employee        VARCHAR(100),
    Username        VARCHAR(50),
    Email           VARCHAR(100),
    Plant           VARCHAR(50),
    Country         VARCHAR(50),
    Manager         VARCHAR(100),
    Valid           VARCHAR(50),
    ManagerUserName VARCHAR(50),
    ETL_LoadDate    DATETIME2    NOT NULL DEFAULT GETUTCDATE(),
    ETL_FileName    VARCHAR(200)
);
GO
