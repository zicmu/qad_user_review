USE [UserReview]
GO

-- ============================================================================
-- Clean Data Warehouse – DELETE all data that depends on source files
--
-- Use this to empty staging + DW tables for a clean load with real data.
-- Order respects foreign keys: children first, then dimensions/staging.
--
-- Excludes: Dim_ReviewPeriod, Ref_ReviewStatus (reference/lookup data).
-- ============================================================================

SET NOCOUNT ON;

-- 1. Review/archive tables (depend on Fact_UserRoleAssignment, Dim_Employee)
DELETE FROM Archive_RoleReview;
DELETE FROM Fact_RoleReview;

-- 2. Fact (depends on Dim_SystemIdentity, Dim_Role)
DELETE FROM Fact_UserRoleAssignment;

-- 3. Bridge (depends on Dim_Employee)
DELETE FROM Bridge_OrgChart;

-- 4. Dimensions fed by ETL (Dim_SystemIdentity depends on Dim_Employee)
DELETE FROM Dim_SystemIdentity;
DELETE FROM Dim_Role;
DELETE FROM Dim_Employee;

-- 5. Canonical staging (no FKs)
DELETE FROM Stg_QAD_Users;
DELETE FROM Stg_QAD_Roles;
DELETE FROM Stg_SAP_Users;
DELETE FROM Stg_SAP_Roles;

-- 6. Raw staging (landing from source files)
DELETE FROM Stg_Raw_SAP_OrgHier;
DELETE FROM Stg_Raw_SAP_Rollen;
DELETE FROM Stg_Raw_QAD_Users2007;
DELETE FROM Stg_Raw_QAD_Users2008;
DELETE FROM Stg_Raw_QAD_DEFRGB;
DELETE FROM Stg_Raw_QAD_ITRS;
DELETE FROM Stg_Raw_QAD_OrgHier;

PRINT 'Data warehouse and staging tables emptied. Ready for full load with real data.';
GO
