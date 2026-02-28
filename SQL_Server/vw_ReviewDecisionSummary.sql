USE [UserReview]
GO

CREATE OR ALTER VIEW vw_ReviewDecisionSummary
AS
SELECT
  si.SystemUsername                                       AS EmployeeUserName,
  emp.FullName                                           AS EmployeeFullName,
  rev.AD_Username                                        AS ReviewerUserName,
  rev.FullName                                           AS ReviewerFullName,
  si.Plant                                               AS Plant,
  emp.Country                                            AS Country,
  r.RoleCode                                             AS [Role],
  r.RoleDescription                                      AS RoleDescription,
  ISNULL(rs.StatusCode, 'Pending')                       AS Decision,
  si.UserType                                            AS EmployeeType,
  CASE WHEN emp.IsActive = 1 THEN 'Active' ELSE 'Inactive' END AS EmployeeStatus,
  fr.ReviewedAt                                          AS DateOfDecision,
  YEAR(rp.StartDate)                                     AS ReviewYear,
  DATEPART(QUARTER, rp.StartDate)                        AS ReviewQuarter
FROM      Fact_RoleReview            fr
JOIN      Fact_UserRoleAssignment    ura ON fr.AssignmentKey      = ura.AssignmentKey
JOIN      Dim_SystemIdentity         si  ON ura.SystemIdentityKey = si.SystemIdentityKey
JOIN      Dim_Employee               emp ON si.EmployeeKey        = emp.EmployeeKey
JOIN      Dim_Employee               rev ON fr.ReviewerKey        = rev.EmployeeKey
JOIN      Dim_Role                   r   ON ura.RoleKey           = r.RoleKey
JOIN      Dim_ReviewPeriod           rp  ON fr.ReviewPeriodKey    = rp.ReviewPeriodKey
LEFT JOIN Ref_ReviewStatus           rs  ON fr.StatusKey          = rs.StatusKey;
GO
