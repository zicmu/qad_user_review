USE [UserReview]
GO

-- ============================================================================
-- CANONICAL STAGING TABLES
-- Normalised view of source data, populated by sp_IngestSourceData from the
-- Stg_Raw_* tables.  SourceSystem distinguishes QAD2007 / QAD2008 / SAP.
-- ============================================================================

IF OBJECT_ID('Stg_QAD_Users', 'U') IS NOT NULL DROP TABLE Stg_QAD_Users;
GO
CREATE TABLE Stg_QAD_Users (
  SystemUsername  VARCHAR(50),
  FullName        VARCHAR(100),
  Plant           VARCHAR(50),
  Email           VARCHAR(100),
  Country         VARCHAR(50),
  ManagerUsername VARCHAR(50),
  UserType        VARCHAR(50),
  Active          VARCHAR(10),
  SourceSystem    VARCHAR(10)  NOT NULL DEFAULT 'QAD',  -- 'QAD2007' | 'QAD2008'
  ETL_LoadDate    DATETIME2    NOT NULL DEFAULT GETUTCDATE(),
  ETL_FileName    VARCHAR(200)
);
GO

IF OBJECT_ID('Stg_QAD_Roles', 'U') IS NOT NULL DROP TABLE Stg_QAD_Roles;
GO
CREATE TABLE Stg_QAD_Roles (
  SystemUsername  VARCHAR(50),
  RoleCode        VARCHAR(50),
  RoleDescription VARCHAR(200),
  SourceSystem    VARCHAR(10)  NOT NULL DEFAULT 'QAD',  -- 'QAD2007' | 'QAD2008'
  ETL_LoadDate    DATETIME2    NOT NULL DEFAULT GETUTCDATE(),
  ETL_FileName    VARCHAR(200)
);
GO

IF OBJECT_ID('Stg_SAP_Users', 'U') IS NOT NULL DROP TABLE Stg_SAP_Users;
GO
CREATE TABLE Stg_SAP_Users (
  SystemUsername  VARCHAR(50),
  FullName        VARCHAR(100),
  Plant           VARCHAR(50),
  Email           VARCHAR(100),
  Country         VARCHAR(50),
  ManagerUsername VARCHAR(50),
  UserType        VARCHAR(50),
  Active          VARCHAR(10),
  SourceSystem    VARCHAR(10)  NOT NULL DEFAULT 'SAP',
  ETL_LoadDate    DATETIME2    NOT NULL DEFAULT GETUTCDATE(),
  ETL_FileName    VARCHAR(200)
);
GO

IF OBJECT_ID('Stg_SAP_Roles', 'U') IS NOT NULL DROP TABLE Stg_SAP_Roles;
GO
CREATE TABLE Stg_SAP_Roles (
  SystemUsername  VARCHAR(50),
  RoleCode        VARCHAR(50),
  RoleDescription VARCHAR(200),
  SourceSystem    VARCHAR(10)  NOT NULL DEFAULT 'SAP',
  ETL_LoadDate    DATETIME2    NOT NULL DEFAULT GETUTCDATE(),
  ETL_FileName    VARCHAR(200)
);
GO
