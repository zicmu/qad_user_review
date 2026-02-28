USE [UserReview]
GO

CREATE PROCEDURE sp_ArchiveReviewPeriod
  @ReviewPeriodKey INT,
  @ArchivedBy      VARCHAR(50),
  @ArchiveReason   VARCHAR(200)
AS
BEGIN
  SET NOCOUNT ON;
  BEGIN TRANSACTION;

    INSERT INTO Archive_RoleReview
      (ReviewKey, ReviewPeriodKey, AssignmentKey, ReviewerKey,
       StatusKey, ReviewerComment, ReviewedAt, LastModifiedAt,
       CreatedAt, UpdatedAt,
       ArchivedAt, ArchivedBy, ArchiveReason)
    SELECT
      ReviewKey, ReviewPeriodKey, AssignmentKey, ReviewerKey,
      StatusKey, ReviewerComment, ReviewedAt, LastModifiedAt,
      CreatedAt, UpdatedAt,
      GETUTCDATE(), @ArchivedBy, @ArchiveReason
    FROM Fact_RoleReview
    WHERE ReviewPeriodKey = @ReviewPeriodKey;

    DELETE FROM Fact_RoleReview
    WHERE ReviewPeriodKey = @ReviewPeriodKey;

    UPDATE Dim_ReviewPeriod
    SET IsActive = 0
    WHERE ReviewPeriodKey = @ReviewPeriodKey;

  COMMIT TRANSACTION;
END;
GO
