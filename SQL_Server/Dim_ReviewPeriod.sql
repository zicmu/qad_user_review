USE [UserReview]
GO

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
