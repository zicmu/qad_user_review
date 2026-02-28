USE [UserReview]
GO

CREATE TABLE Bridge_OrgChart (
  OrgChartKey  INT IDENTITY(1,1) PRIMARY KEY,
  EmployeeKey  INT          NOT NULL REFERENCES Dim_Employee(EmployeeKey),
  ReviewerKey  INT          NOT NULL REFERENCES Dim_Employee(EmployeeKey),
  SourceSystem VARCHAR(10)  NOT NULL,
  ValidFrom    DATE         NOT NULL,
  ValidTo      DATE,
  IsActive     BIT          NOT NULL DEFAULT 1,
  ETL_LoadDate DATETIME2    NOT NULL DEFAULT GETUTCDATE()
);
GO
