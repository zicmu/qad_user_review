USE [UserReview]
GO

-- ============================================================================
-- sp_InsertReviewPeriod
-- Inserts a new row into Dim_ReviewPeriod and optionally makes it the active
-- period (sets all other rows to IsActive = 0). Use this to populate the
-- "remaining" fields: PeriodName, PeriodType, StartDate, EndDate, IsActive.
-- LastUpdateDate can be set later from the Data Import page or via UPDATE.
-- ============================================================================

CREATE OR ALTER PROCEDURE dbo.sp_InsertReviewPeriod
  @PeriodName     VARCHAR(50),
  @PeriodType     VARCHAR(20),   -- 'Monthly' | 'Quarterly'
  @StartDate      DATE,
  @EndDate        DATE,
  @SetAsActive    BIT = 1,      -- If 1, set this period as active (IsActive=1) and all others to 0
  @LastUpdateDate DATETIME2 = NULL
AS
BEGIN
  SET NOCOUNT ON;

  IF @SetAsActive = 1
    UPDATE Dim_ReviewPeriod SET IsActive = 0;

  INSERT INTO Dim_ReviewPeriod (PeriodName, PeriodType, StartDate, EndDate, IsActive, LastUpdateDate)
  VALUES (@PeriodName, @PeriodType, @StartDate, @EndDate, @SetAsActive, @LastUpdateDate);
END;
GO
