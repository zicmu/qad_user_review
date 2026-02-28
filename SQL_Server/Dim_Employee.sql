USE [UserReview]
GO

CREATE TABLE Dim_Employee (
  EmployeeKey   INT IDENTITY(1,1) PRIMARY KEY,
  AD_Username   VARCHAR(50)  NOT NULL UNIQUE,
  FullName      VARCHAR(100) NOT NULL,
  Email         VARCHAR(100),
  Country       VARCHAR(50),
  IsActive      BIT          NOT NULL DEFAULT 1,
  CreatedAt     DATETIME2    NOT NULL DEFAULT GETUTCDATE(),
  UpdatedAt     DATETIME2    NOT NULL DEFAULT GETUTCDATE()
);
GO
