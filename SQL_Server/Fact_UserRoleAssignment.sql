USE [UserReview]
GO

CREATE TABLE Fact_UserRoleAssignment (
  AssignmentKey     INT IDENTITY(1,1) PRIMARY KEY,
  SystemIdentityKey INT          NOT NULL REFERENCES Dim_SystemIdentity(SystemIdentityKey),
  RoleKey           INT          NOT NULL REFERENCES Dim_Role(RoleKey),
  SourceSystem      VARCHAR(10)  NOT NULL,
  IsActive          BIT          NOT NULL DEFAULT 1,
  ETL_LoadDate      DATETIME2    NOT NULL DEFAULT GETUTCDATE(),
  CONSTRAINT UQ_Assignment UNIQUE (SystemIdentityKey, RoleKey)
);
GO
