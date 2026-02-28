USE [UserReview]
GO

-- QAD staging
CREATE TABLE Stg_QAD_Users (
  SystemUsername  VARCHAR(50),
  FullName        VARCHAR(100),
  Plant           VARCHAR(50),
  Email           VARCHAR(100),
  Country         VARCHAR(50),
  ManagerUsername VARCHAR(50),
  UserType        VARCHAR(50),
  Active          VARCHAR(10),
  ETL_LoadDate    DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
  ETL_FileName    VARCHAR(200)
);
GO

CREATE TABLE Stg_QAD_Roles (
  SystemUsername  VARCHAR(50),
  RoleCode        VARCHAR(50),
  RoleDescription VARCHAR(200),
  ETL_LoadDate    DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
  ETL_FileName    VARCHAR(200)
);
GO

-- SAP staging
CREATE TABLE Stg_SAP_Users (
  SystemUsername  VARCHAR(50),
  FullName        VARCHAR(100),
  Plant           VARCHAR(50),
  Email           VARCHAR(100),
  Country         VARCHAR(50),
  ManagerUsername VARCHAR(50),
  UserType        VARCHAR(50),
  Active          VARCHAR(10),
  ETL_LoadDate    DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
  ETL_FileName    VARCHAR(200)
);
GO

CREATE TABLE Stg_SAP_Roles (
  SystemUsername  VARCHAR(50),
  RoleCode        VARCHAR(50),
  RoleDescription VARCHAR(200),
  ETL_LoadDate    DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
  ETL_FileName    VARCHAR(200)
);
GO
