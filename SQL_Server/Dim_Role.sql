USE [UserReview]
GO

CREATE TABLE Dim_Role (
  RoleKey         INT IDENTITY(1,1) PRIMARY KEY,
  RoleCode        VARCHAR(50)  NOT NULL,
  RoleDescription VARCHAR(200),
  SourceSystem    VARCHAR(10)  NOT NULL, -- 'QAD' | 'SAP'
  IsActive        BIT          NOT NULL DEFAULT 1,
  CONSTRAINT UQ_Role UNIQUE (RoleCode, SourceSystem)
);
GO
