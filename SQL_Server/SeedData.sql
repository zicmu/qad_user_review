USE [UserReview]
GO

SET NOCOUNT ON;

-- ============================================================================
-- SEED DATA: Realistic sample data for development and testing
-- ============================================================================

-- Dim_Employee: 20 employees (including 4 managers)
SET IDENTITY_INSERT Dim_Employee ON;
INSERT INTO Dim_Employee (EmployeeKey, AD_Username, FullName, Email, Country, IsActive) VALUES
  (1,  'j.mueller',    'Jan Mueller',        'jan.mueller@essex.com',      'Germany',  1),
  (2,  'k.schmidt',    'Katrin Schmidt',      'katrin.schmidt@essex.com',   'Germany',  1),
  (3,  'p.rossi',      'Paolo Rossi',         'paolo.rossi@essex.com',      'Italy',    1),
  (4,  'a.bianchi',    'Anna Bianchi',        'anna.bianchi@essex.com',     'Italy',    1),
  (5,  'm.weber',      'Michael Weber',       'michael.weber@essex.com',    'Germany',  1),
  (6,  'l.ferrari',    'Luca Ferrari',        'luca.ferrari@essex.com',     'Italy',    1),
  (7,  's.hoffmann',   'Stefan Hoffmann',     'stefan.hoffmann@essex.com',  'Germany',  1),
  (8,  'e.marjas',     'Elena Marjas',        'elena.marjas@essex.com',     'Italy',    1),
  (9,  'f.krueger',    'Frank Krueger',       'frank.krueger@essex.com',    'Germany',  1),
  (10, 'g.conti',      'Giulia Conti',        'giulia.conti@essex.com',     'Italy',    1),
  (11, 'r.braun',      'Robert Braun',        'robert.braun@essex.com',     'Germany',  1),
  (12, 'm.russo',      'Marco Russo',         'marco.russo@essex.com',      'Italy',    1),
  (13, 'h.becker',     'Hans Becker',         'hans.becker@essex.com',      'Germany',  1),
  (14, 'c.lombardi',   'Chiara Lombardi',     'chiara.lombardi@essex.com',  'Italy',    1),
  (15, 't.wagner',     'Thomas Wagner',       'thomas.wagner@essex.com',    'Germany',  1),
  (16, 'a.greco',      'Alessandro Greco',    'alessandro.greco@essex.com', 'Italy',    1),
  (17, 'd.fischer',    'Daniela Fischer',     'daniela.fischer@essex.com',  'Germany',  0),
  (18, 's.romano',     'Stefano Romano',      'stefano.romano@essex.com',   'Italy',    0),
  (19, 'n.schwarz',    'Nina Schwarz',        'nina.schwarz@essex.com',     'Germany',  1),
  (20, 'v.moretti',    'Valentina Moretti',   'valentina.moretti@essex.com','Italy',    1);
SET IDENTITY_INSERT Dim_Employee OFF;
GO

-- Dim_SystemIdentity: QAD + SAP identities
-- Managers (keys 1-4) are reviewers; employees (5-20) have system access
SET IDENTITY_INSERT Dim_SystemIdentity ON;
INSERT INTO Dim_SystemIdentity (SystemIdentityKey, EmployeeKey, SystemUsername, SourceSystem, Plant, UserType, IsActive) VALUES
  -- QAD identities (German plant)
  (1,  5,  'MWEBER',     'QAD', 'DE-HAM', 'Dialog',   1),
  (2,  7,  'SHOFFMANN',  'QAD', 'DE-HAM', 'Dialog',   1),
  (3,  9,  'FKRUEGER',   'QAD', 'DE-HAM', 'Dialog',   1),
  (4,  11, 'RBRAUN',     'QAD', 'DE-HAM', 'Dialog',   1),
  (5,  13, 'HBECKER',    'QAD', 'DE-HAM', 'Dialog',   1),
  (6,  15, 'TWAGNER',    'QAD', 'DE-HAM', 'System',   1),
  (7,  17, 'DFISCHER',   'QAD', 'DE-HAM', 'Dialog',   0),
  (8,  19, 'NSCHWARZ',   'QAD', 'DE-HAM', 'Dialog',   1),
  -- QAD identities (Italian plant)
  (9,  6,  'LFERRARI',   'QAD', 'IT-MIL', 'Dialog',   1),
  (10, 8,  'EMARJAS',    'QAD', 'IT-MIL', 'Dialog',   1),
  (11, 10, 'GCONTI',     'QAD', 'IT-MIL', 'Dialog',   1),
  (12, 12, 'MRUSSO',     'QAD', 'IT-MIL', 'Dialog',   1),
  (13, 14, 'CLOMBARDI',  'QAD', 'IT-MIL', 'System',   1),
  (14, 16, 'AGRECO',     'QAD', 'IT-MIL', 'Dialog',   1),
  (15, 18, 'SROMANO',    'QAD', 'IT-MIL', 'Dialog',   0),
  (16, 20, 'VMORETTI',   'QAD', 'IT-MIL', 'Dialog',   1),
  -- SAP identities (some employees also have SAP access)
  (17, 5,  'DE_MWEBER',  'SAP', 'DE-HAM', 'Dialog',   1),
  (18, 7,  'DE_SHOFF',   'SAP', 'DE-HAM', 'Dialog',   1),
  (19, 9,  'DE_FKRUE',   'SAP', 'DE-HAM', 'Dialog',   1),
  (20, 6,  'IT_LFERR',   'SAP', 'IT-MIL', 'Dialog',   1),
  (21, 8,  'IT_EMARJ',   'SAP', 'IT-MIL', 'Dialog',   1),
  (22, 10, 'IT_GCONT',   'SAP', 'IT-MIL', 'Dialog',   1);
SET IDENTITY_INSERT Dim_SystemIdentity OFF;
GO

-- Dim_Role: QAD and SAP roles
SET IDENTITY_INSERT Dim_Role ON;
INSERT INTO Dim_Role (RoleKey, RoleCode, RoleDescription, SourceSystem, IsActive) VALUES
  -- QAD roles
  (1,  'QAD_PURCH',     'Purchasing',               'QAD', 1),
  (2,  'QAD_SALES',     'Sales Order Management',   'QAD', 1),
  (3,  'QAD_WHSE',      'Warehouse Management',     'QAD', 1),
  (4,  'QAD_FIN',       'Financial Accounting',     'QAD', 1),
  (5,  'QAD_PROD',      'Production Planning',      'QAD', 1),
  (6,  'QAD_ADMIN',     'System Administration',    'QAD', 1),
  (7,  'QAD_INV',       'Inventory Management',     'QAD', 1),
  (8,  'QAD_QC',        'Quality Control',          'QAD', 1),
  -- SAP roles
  (9,  'SAP_MM_BUYER',  'Material Management Buyer','SAP', 1),
  (10, 'SAP_SD_SALES',  'Sales & Distribution',     'SAP', 1),
  (11, 'SAP_FI_GL',     'FI General Ledger',        'SAP', 1),
  (12, 'SAP_PP_PLAN',   'Production Planning',      'SAP', 1),
  (13, 'SAP_WM_OPER',   'Warehouse Operations',     'SAP', 1),
  (14, 'SAP_BASIS',     'SAP Basis Admin',          'SAP', 1);
SET IDENTITY_INSERT Dim_Role OFF;
GO

-- Dim_Menu: menus per QAD role
INSERT INTO Dim_Menu (RoleKey, MenuCode, MenuDescription) VALUES
  (1, 'PO-CREATE',   'Create Purchase Order'),
  (1, 'PO-APPROVE',  'Approve Purchase Order'),
  (1, 'VENDOR-MAINT','Vendor Maintenance'),
  (2, 'SO-CREATE',   'Create Sales Order'),
  (2, 'SO-MODIFY',   'Modify Sales Order'),
  (3, 'WH-RECEIPT',  'Warehouse Receipt'),
  (3, 'WH-SHIP',     'Warehouse Shipment'),
  (4, 'GL-POST',     'GL Posting'),
  (4, 'AP-INVOICE',  'AP Invoice Entry'),
  (5, 'MRP-RUN',     'Run MRP'),
  (5, 'WO-CREATE',   'Create Work Order'),
  (6, 'SYS-CONFIG',  'System Configuration'),
  (6, 'USER-MGMT',   'User Management'),
  (7, 'INV-COUNT',   'Inventory Count'),
  (7, 'INV-ADJUST',  'Inventory Adjustment'),
  (8, 'QC-INSPECT',  'Quality Inspection'),
  (8, 'QC-HOLD',     'Quality Hold');
GO

-- Bridge_OrgChart: manager assignments
-- Managers: j.mueller (1) manages DE QAD team, k.schmidt (2) manages DE SAP team
-- p.rossi (3) manages IT QAD team, a.bianchi (4) manages IT SAP team
INSERT INTO Bridge_OrgChart (EmployeeKey, ReviewerKey, SourceSystem, ValidFrom, ValidTo, IsActive) VALUES
  -- j.mueller manages DE QAD employees
  (5,  1, 'QAD', '2024-01-01', NULL, 1),
  (7,  1, 'QAD', '2024-01-01', NULL, 1),
  (9,  1, 'QAD', '2024-01-01', NULL, 1),
  (11, 1, 'QAD', '2024-01-01', NULL, 1),
  (13, 1, 'QAD', '2024-01-01', NULL, 1),
  (15, 1, 'QAD', '2024-01-01', NULL, 1),
  (17, 1, 'QAD', '2024-01-01', NULL, 1),
  (19, 1, 'QAD', '2024-01-01', NULL, 1),
  -- k.schmidt manages DE SAP employees
  (5,  2, 'SAP', '2024-01-01', NULL, 1),
  (7,  2, 'SAP', '2024-01-01', NULL, 1),
  (9,  2, 'SAP', '2024-01-01', NULL, 1),
  -- p.rossi manages IT QAD employees
  (6,  3, 'QAD', '2024-01-01', NULL, 1),
  (8,  3, 'QAD', '2024-01-01', NULL, 1),
  (10, 3, 'QAD', '2024-01-01', NULL, 1),
  (12, 3, 'QAD', '2024-01-01', NULL, 1),
  (14, 3, 'QAD', '2024-01-01', NULL, 1),
  (16, 3, 'QAD', '2024-01-01', NULL, 1),
  (18, 3, 'QAD', '2024-01-01', NULL, 1),
  (20, 3, 'QAD', '2024-01-01', NULL, 1),
  -- a.bianchi manages IT SAP employees
  (6,  4, 'SAP', '2024-01-01', NULL, 1),
  (8,  4, 'SAP', '2024-01-01', NULL, 1),
  (10, 4, 'SAP', '2024-01-01', NULL, 1);
GO

-- Dim_ReviewPeriod: current active + one archived
SET IDENTITY_INSERT Dim_ReviewPeriod ON;
INSERT INTO Dim_ReviewPeriod (ReviewPeriodKey, PeriodName, PeriodType, StartDate, EndDate, IsActive) VALUES
  (1, 'Q4 2025', 'Quarterly', '2025-10-01', '2025-12-31', 0),
  (2, 'Q1 2026', 'Quarterly', '2026-01-01', '2026-03-31', 1);
SET IDENTITY_INSERT Dim_ReviewPeriod OFF;
GO

-- Fact_UserRoleAssignment: assign roles to system identities
SET IDENTITY_INSERT Fact_UserRoleAssignment ON;
INSERT INTO Fact_UserRoleAssignment (AssignmentKey, SystemIdentityKey, RoleKey, SourceSystem, IsActive) VALUES
  -- QAD DE employees
  (1,  1,  1, 'QAD', 1),  -- MWEBER  -> Purchasing
  (2,  1,  7, 'QAD', 1),  -- MWEBER  -> Inventory
  (3,  2,  2, 'QAD', 1),  -- SHOFFMANN -> Sales
  (4,  3,  5, 'QAD', 1),  -- FKRUEGER -> Production
  (5,  3,  8, 'QAD', 1),  -- FKRUEGER -> Quality Control
  (6,  4,  3, 'QAD', 1),  -- RBRAUN -> Warehouse
  (7,  5,  4, 'QAD', 1),  -- HBECKER -> Finance
  (8,  6,  6, 'QAD', 1),  -- TWAGNER -> Admin
  (9,  7,  1, 'QAD', 0),  -- DFISCHER -> Purchasing (inactive)
  (10, 8,  2, 'QAD', 1),  -- NSCHWARZ -> Sales
  -- QAD IT employees
  (11, 9,  1, 'QAD', 1),  -- LFERRARI -> Purchasing
  (12, 9,  3, 'QAD', 1),  -- LFERRARI -> Warehouse
  (13, 10, 5, 'QAD', 1),  -- EMARJAS -> Production
  (14, 11, 2, 'QAD', 1),  -- GCONTI -> Sales
  (15, 12, 4, 'QAD', 1),  -- MRUSSO -> Finance
  (16, 13, 6, 'QAD', 1),  -- CLOMBARDI -> Admin
  (17, 14, 7, 'QAD', 1),  -- AGRECO -> Inventory
  (18, 14, 8, 'QAD', 1),  -- AGRECO -> Quality Control
  (19, 15, 1, 'QAD', 0),  -- SROMANO -> Purchasing (inactive)
  (20, 16, 2, 'QAD', 1),  -- VMORETTI -> Sales
  -- SAP identities
  (21, 17, 9,  'SAP', 1), -- DE_MWEBER -> MM Buyer
  (22, 18, 10, 'SAP', 1), -- DE_SHOFF -> Sales & Dist
  (23, 19, 11, 'SAP', 1), -- DE_FKRUE -> FI GL
  (24, 20, 12, 'SAP', 1), -- IT_LFERR -> PP Plan
  (25, 21, 13, 'SAP', 1), -- IT_EMARJ -> WM Oper
  (26, 22, 10, 'SAP', 1); -- IT_GCONT -> Sales & Dist
SET IDENTITY_INSERT Fact_UserRoleAssignment OFF;
GO

-- Fact_RoleReview: active review period (Q1 2026) - mix of pending, approved, disabled
-- ReviewPeriodKey=2 is the active period
-- StatusKey: 1=Pending, 2=Approved, 3=Disabled, NULL=Pending
INSERT INTO Fact_RoleReview (ReviewPeriodKey, AssignmentKey, ReviewerKey, StatusKey, ReviewerComment, ReviewedAt, LastModifiedAt, CreatedAt, UpdatedAt) VALUES
  -- j.mueller (1) reviewing DE QAD employees
  (2, 1,  1, 2,    'Access confirmed',           '2026-02-10', '2026-02-10', '2026-01-01', '2026-02-10'),
  (2, 2,  1, 2,    'Inventory access needed',    '2026-02-10', '2026-02-10', '2026-01-01', '2026-02-10'),
  (2, 3,  1, NULL,  NULL,                         NULL,          NULL,         '2026-01-01', '2026-01-01'),
  (2, 4,  1, NULL,  NULL,                         NULL,          NULL,         '2026-01-01', '2026-01-01'),
  (2, 5,  1, 3,    'User moved to new dept',     '2026-02-15', '2026-02-15', '2026-01-01', '2026-02-15'),
  (2, 6,  1, 2,    'OK',                         '2026-02-12', '2026-02-12', '2026-01-01', '2026-02-12'),
  (2, 7,  1, NULL,  NULL,                         NULL,          NULL,         '2026-01-01', '2026-01-01'),
  (2, 8,  1, 2,    'Admin access required',      '2026-02-11', '2026-02-11', '2026-01-01', '2026-02-11'),
  (2, 10, 1, NULL,  NULL,                         NULL,          NULL,         '2026-01-01', '2026-01-01'),
  -- p.rossi (3) reviewing IT QAD employees
  (2, 11, 3, 2,    'Confermato',                 '2026-02-08', '2026-02-08', '2026-01-01', '2026-02-08'),
  (2, 12, 3, 2,    'Confermato',                 '2026-02-08', '2026-02-08', '2026-01-01', '2026-02-08'),
  (2, 13, 3, NULL,  NULL,                         NULL,          NULL,         '2026-01-01', '2026-01-01'),
  (2, 14, 3, 2,    'OK',                         '2026-02-09', '2026-02-09', '2026-01-01', '2026-02-09'),
  (2, 15, 3, NULL,  NULL,                         NULL,          NULL,         '2026-01-01', '2026-01-01'),
  (2, 16, 3, 3,    'Utente da disabilitare',     '2026-02-20', '2026-02-20', '2026-01-01', '2026-02-20'),
  (2, 17, 3, NULL,  NULL,                         NULL,          NULL,         '2026-01-01', '2026-01-01'),
  (2, 18, 3, 2,    'QC access needed',           '2026-02-14', '2026-02-14', '2026-01-01', '2026-02-14'),
  (2, 20, 3, NULL,  NULL,                         NULL,          NULL,         '2026-01-01', '2026-01-01'),
  -- k.schmidt (2) reviewing DE SAP employees
  (2, 21, 2, 2,    'SAP access confirmed',       '2026-02-18', '2026-02-18', '2026-01-01', '2026-02-18'),
  (2, 22, 2, NULL,  NULL,                         NULL,          NULL,         '2026-01-01', '2026-01-01'),
  (2, 23, 2, 2,    'FI access required',         '2026-02-19', '2026-02-19', '2026-01-01', '2026-02-19'),
  -- a.bianchi (4) reviewing IT SAP employees
  (2, 24, 4, NULL,  NULL,                         NULL,          NULL,         '2026-01-01', '2026-01-01'),
  (2, 25, 4, 2,    'Warehouse access OK',        '2026-02-22', '2026-02-22', '2026-01-01', '2026-02-22'),
  (2, 26, 4, NULL,  NULL,                         NULL,          NULL,         '2026-01-01', '2026-01-01');
GO

-- Archive_RoleReview: archived Q4 2025 data
INSERT INTO Archive_RoleReview (ReviewKey, ReviewPeriodKey, AssignmentKey, ReviewerKey, StatusKey, ReviewerComment, ReviewedAt, LastModifiedAt, CreatedAt, UpdatedAt, ArchivedAt, ArchivedBy, ArchiveReason) VALUES
  (1001, 1, 1,  1, 2, 'Approved for Q4',   '2025-11-15', '2025-11-15', '2025-10-01', '2025-11-15', '2025-12-31', 'j.mueller',  'Q4 2025 period closed'),
  (1002, 1, 3,  1, 2, 'Sales access OK',   '2025-11-16', '2025-11-16', '2025-10-01', '2025-11-16', '2025-12-31', 'j.mueller',  'Q4 2025 period closed'),
  (1003, 1, 6,  1, 2, 'Warehouse OK',      '2025-11-17', '2025-11-17', '2025-10-01', '2025-11-17', '2025-12-31', 'j.mueller',  'Q4 2025 period closed'),
  (1004, 1, 11, 3, 2, 'Confermato',         '2025-11-10', '2025-11-10', '2025-10-01', '2025-11-10', '2025-12-31', 'p.rossi',    'Q4 2025 period closed'),
  (1005, 1, 14, 3, 2, 'OK',                 '2025-11-11', '2025-11-11', '2025-10-01', '2025-11-11', '2025-12-31', 'p.rossi',    'Q4 2025 period closed'),
  (1006, 1, 21, 2, 2, 'SAP access valid',   '2025-11-20', '2025-11-20', '2025-10-01', '2025-11-20', '2025-12-31', 'k.schmidt',  'Q4 2025 period closed'),
  (1007, 1, 25, 4, 3, 'Access revoked',     '2025-11-22', '2025-11-22', '2025-10-01', '2025-11-22', '2025-12-31', 'a.bianchi',  'Q4 2025 period closed');
GO

-- Staging tables: sample import data
INSERT INTO Stg_QAD_Users (SystemUsername, FullName, Plant, Email, Country, ManagerUsername, UserType, Active, ETL_FileName) VALUES
  ('MWEBER',    'Michael Weber',     'DE-HAM', 'michael.weber@essex.com',    'Germany', 'JMUELLER', 'Dialog', 'Yes', 'QAD_Users_2026Q1.xlsx'),
  ('SHOFFMANN', 'Stefan Hoffmann',   'DE-HAM', 'stefan.hoffmann@essex.com',  'Germany', 'JMUELLER', 'Dialog', 'Yes', 'QAD_Users_2026Q1.xlsx'),
  ('LFERRARI',  'Luca Ferrari',      'IT-MIL', 'luca.ferrari@essex.com',     'Italy',   'PROSSI',   'Dialog', 'Yes', 'QAD_Users_2026Q1.xlsx'),
  ('EMARJAS',   'Elena Marjas',      'IT-MIL', 'elena.marjas@essex.com',     'Italy',   'PROSSI',   'Dialog', 'Yes', 'QAD_Users_2026Q1.xlsx');
GO

INSERT INTO Stg_QAD_Roles (SystemUsername, RoleCode, RoleDescription, ETL_FileName) VALUES
  ('MWEBER',    'QAD_PURCH', 'Purchasing',           'QAD_Roles_2026Q1.xlsx'),
  ('MWEBER',    'QAD_INV',   'Inventory Management', 'QAD_Roles_2026Q1.xlsx'),
  ('SHOFFMANN', 'QAD_SALES', 'Sales Order Mgmt',     'QAD_Roles_2026Q1.xlsx'),
  ('LFERRARI',  'QAD_PURCH', 'Purchasing',           'QAD_Roles_2026Q1.xlsx'),
  ('EMARJAS',   'QAD_PROD',  'Production Planning',  'QAD_Roles_2026Q1.xlsx');
GO

PRINT 'Seed data loaded successfully.';
GO
