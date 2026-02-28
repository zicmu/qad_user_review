USE [UserReview]
GO

CREATE TABLE Dim_Menu (
  MenuKey         INT IDENTITY(1,1) PRIMARY KEY,
  RoleKey         INT          NOT NULL REFERENCES Dim_Role(RoleKey),
  MenuCode        VARCHAR(50)  NOT NULL,
  MenuDescription VARCHAR(200)
);
GO
