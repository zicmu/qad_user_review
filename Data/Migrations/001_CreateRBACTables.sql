-- ============================================================
-- RBAC Migration: Create Role-Based Access Control Tables
-- Run this script against the QAD User Review database
-- ============================================================

-- 1. Ref_AppRole
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Ref_AppRole')
BEGIN
    CREATE TABLE Ref_AppRole (
        AppRoleKey      INT IDENTITY(1,1) PRIMARY KEY,
        RoleCode        VARCHAR(20)  NOT NULL UNIQUE,
        RoleDescription VARCHAR(100)
    );

    INSERT INTO Ref_AppRole (RoleCode, RoleDescription) VALUES
        ('SuperAdmin', 'Full access including role management'),
        ('Admin',      'Manages users, imports, and employee-reviewer assignments'),
        ('Auditor',    'Read-only unfiltered access to all reviews'),
        ('Reviewer',   'Can review and submit decisions for own employees only');

    PRINT 'Created and seeded Ref_AppRole';
END
GO

-- 2. Ref_AppFeature
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Ref_AppFeature')
BEGIN
    CREATE TABLE Ref_AppFeature (
        FeatureKey         INT IDENTITY(1,1) PRIMARY KEY,
        FeatureCode        VARCHAR(50)  NOT NULL UNIQUE,
        FeatureDescription VARCHAR(100)
    );

    INSERT INTO Ref_AppFeature (FeatureCode, FeatureDescription) VALUES
        ('ReviewAll',              'View reviews for all employees unfiltered'),
        ('ReviewOwn',              'View reviews scoped to own employees'),
        ('SubmitDecision',         'Submit Approved/Disabled decision + comment'),
        ('ManageEmployeeReviewer', 'View and edit employee-reviewer assignments'),
        ('Import',                 'Upload and process Excel files'),
        ('Reports',                'View audit history and reporting'),
        ('ManageUsers',            'Add/edit/deactivate app users and their roles'),
        ('ManageRoles',            'Edit role-feature permission assignments'),
        ('ManagePeriod',           'Open and close review periods, trigger archive');

    PRINT 'Created and seeded Ref_AppFeature';
END
GO

-- 3. Bridge_RoleFeature
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Bridge_RoleFeature')
BEGIN
    CREATE TABLE Bridge_RoleFeature (
        RoleFeatureKey INT IDENTITY(1,1) PRIMARY KEY,
        AppRoleKey     INT NOT NULL REFERENCES Ref_AppRole(AppRoleKey),
        FeatureKey     INT NOT NULL REFERENCES Ref_AppFeature(FeatureKey),
        CanAccess      BIT NOT NULL DEFAULT 1,
        CONSTRAINT UQ_RoleFeature UNIQUE (AppRoleKey, FeatureKey)
    );

    -- SuperAdmin (1): all features granted
    INSERT INTO Bridge_RoleFeature (AppRoleKey, FeatureKey, CanAccess) VALUES
        (1, 1, 1), (1, 2, 1), (1, 3, 1), (1, 4, 1), (1, 5, 1),
        (1, 6, 1), (1, 7, 1), (1, 8, 1), (1, 9, 1);

    -- Admin (2): all except ManageRoles (8)
    INSERT INTO Bridge_RoleFeature (AppRoleKey, FeatureKey, CanAccess) VALUES
        (2, 1, 1), (2, 2, 1), (2, 3, 1), (2, 4, 1), (2, 5, 1),
        (2, 6, 1), (2, 7, 1), (2, 8, 0), (2, 9, 1);

    -- Auditor (3): ReviewAll, ReviewOwn, Reports only
    INSERT INTO Bridge_RoleFeature (AppRoleKey, FeatureKey, CanAccess) VALUES
        (3, 1, 1), (3, 2, 1), (3, 3, 0), (3, 4, 0), (3, 5, 0),
        (3, 6, 1), (3, 7, 0), (3, 8, 0), (3, 9, 0);

    -- Reviewer (4): ReviewOwn, SubmitDecision only
    INSERT INTO Bridge_RoleFeature (AppRoleKey, FeatureKey, CanAccess) VALUES
        (4, 1, 0), (4, 2, 1), (4, 3, 1), (4, 4, 0), (4, 5, 0),
        (4, 6, 0), (4, 7, 0), (4, 8, 0), (4, 9, 0);

    PRINT 'Created and seeded Bridge_RoleFeature';
END
GO

-- 4. App_User
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'App_User')
BEGIN
    CREATE TABLE App_User (
        AppUserKey  INT IDENTITY(1,1) PRIMARY KEY,
        EmployeeKey INT NOT NULL REFERENCES Dim_Employee(EmployeeKey),
        AppRoleKey  INT NOT NULL REFERENCES Ref_AppRole(AppRoleKey),
        IsActive    BIT       NOT NULL DEFAULT 1,
        CreatedAt   DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt   DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        AssignedBy  INT       NOT NULL REFERENCES Dim_Employee(EmployeeKey)
    );

    PRINT 'Created App_User';
END
GO

PRINT 'RBAC migration complete.';
GO
