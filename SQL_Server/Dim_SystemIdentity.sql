USE [UserReview]
GO

CREATE TABLE Dim_SystemIdentity (
  SystemIdentityKey INT IDENTITY(1,1) PRIMARY KEY,
  EmployeeKey       INT          NOT NULL REFERENCES Dim_Employee(EmployeeKey),
  SystemUsername     VARCHAR(50)  NOT NULL,
  SourceSystem      VARCHAR(10)  NOT NULL, -- 'QAD2007' | 'QAD2008' | 'SAP'
  Plant             VARCHAR(50),
  UserType          VARCHAR(50),
  IsActive          BIT          NOT NULL DEFAULT 1,
  ETL_LoadDate      DATETIME2    NOT NULL DEFAULT GETUTCDATE(),
  CONSTRAINT UQ_SystemIdentity UNIQUE (SystemUsername, SourceSystem)
);
GO
