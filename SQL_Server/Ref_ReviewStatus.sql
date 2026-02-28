USE [UserReview]
GO

CREATE TABLE Ref_ReviewStatus (
  StatusKey         INT IDENTITY(1,1) PRIMARY KEY,
  StatusCode        VARCHAR(20)  NOT NULL UNIQUE,
  StatusDescription VARCHAR(100)
);
GO

INSERT INTO Ref_ReviewStatus (StatusCode, StatusDescription) VALUES
  ('Pending',  'Awaiting reviewer decision'),
  ('Approved', 'Access approved for next period'),
  ('Disabled', 'Access to be revoked');
GO
