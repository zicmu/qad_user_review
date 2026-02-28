USE [UserReview]
GO

CREATE TABLE Fact_RoleReview (
  ReviewKey       INT IDENTITY(1,1) PRIMARY KEY,
  ReviewPeriodKey INT          NOT NULL REFERENCES Dim_ReviewPeriod(ReviewPeriodKey),
  AssignmentKey   INT          NOT NULL REFERENCES Fact_UserRoleAssignment(AssignmentKey),
  ReviewerKey     INT          NOT NULL REFERENCES Dim_Employee(EmployeeKey),
  StatusKey       INT          REFERENCES Ref_ReviewStatus(StatusKey), -- NULL = Pending
  ReviewerComment VARCHAR(500),
  ReviewedAt      DATETIME2,
  LastModifiedAt  DATETIME2,
  CreatedAt       DATETIME2    NOT NULL DEFAULT GETUTCDATE(),
  UpdatedAt       DATETIME2    NOT NULL DEFAULT GETUTCDATE()
);
GO
