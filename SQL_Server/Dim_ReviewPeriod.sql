USE [UserReview]
GO

-- ============================================================================
-- Dim_ReviewPeriod
-- Defines review windows (e.g. quarterly). Exactly one row should have IsActive=1;
-- that row is used for the current review list and for LastUpdateDate from imports.
--
-- Column population:
--   ReviewPeriodKey  : IDENTITY (auto).
--   PeriodName       : Human-readable label, e.g. 'Q1 2026'. Set when inserting.
--   PeriodType       : 'Monthly' or 'Quarterly'. Set when inserting.
--   StartDate        : Period start (inclusive). Set when inserting.
--   EndDate          : Period end (inclusive). Set when inserting.
--   IsActive         : 1 = current period (only one should be 1). Set when inserting; updated when archiving/activating.
--   LastUpdateDate   : When source data was last extracted for this period. Set from the Data Import page (Process Data) or manually.
--
-- To add a new period: run sp_InsertReviewPeriod (see below) or INSERT then UPDATE IsActive as needed.
-- ============================================================================

CREATE TABLE Dim_ReviewPeriod (
  ReviewPeriodKey INT IDENTITY(1,1) PRIMARY KEY,
  PeriodName      VARCHAR(50)  NOT NULL,
  PeriodType      VARCHAR(20)  NOT NULL, -- 'Monthly' | 'Quarterly'
  StartDate       DATE         NOT NULL,
  EndDate         DATE         NOT NULL,
  IsActive        BIT          NOT NULL DEFAULT 1,
  LastUpdateDate  DATETIME2    NULL
);
GO
