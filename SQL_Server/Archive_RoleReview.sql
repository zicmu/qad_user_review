USE [UserReview]
GO

CREATE TABLE Archive_RoleReview (
  ArchiveKey      INT IDENTITY(1,1) PRIMARY KEY,
  ReviewKey       INT          NOT NULL,
  ReviewPeriodKey INT          NOT NULL REFERENCES Dim_ReviewPeriod(ReviewPeriodKey),
  AssignmentKey   INT          NOT NULL REFERENCES Fact_UserRoleAssignment(AssignmentKey),
  ReviewerKey     INT          NOT NULL REFERENCES Dim_Employee(EmployeeKey),
  StatusKey       INT          REFERENCES Ref_ReviewStatus(StatusKey),
  ReviewerComment VARCHAR(500),
  ReviewedAt      DATETIME2,
  LastModifiedAt  DATETIME2,
  CreatedAt       DATETIME2,
  UpdatedAt       DATETIME2,
  ArchivedAt      DATETIME2    NOT NULL DEFAULT GETUTCDATE(),
  ArchivedBy      VARCHAR(50)  NOT NULL,
  ArchiveReason   VARCHAR(200)
);
GO
