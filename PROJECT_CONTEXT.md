# Project Context — QAD User Review

## Overview

This project is a refactor of a legacy ASP.NET MVC application originally written by a junior developer.
The goal is to modernize the codebase into a clean, maintainable, and scalable ASP.NET Core Web API.

---

## Current State (v1.1.0 — Post Architecture Review)

| Property | Detail |
|---|---|
| Framework | ASP.NET Core MVC (.NET 6.0) |
| Language | C# (nullable reference types enabled, implicit usings) |
| App size | 6 controllers, 5 models, 4 ViewModels, 17 views |
| Database | SQL Server (`UserReview` database on `ARPR-ITSVR04.ge.eu.spsx.com`) |
| ORM | Entity Framework Core 6.0.9 (SQL Server provider) |
| Auth | Windows Authentication (IIS) — no middleware, relies on `User.Identity.Name` |
| Tests | None |
| Repository | [github.com/zicmu/qad_user_review](https://github.com/zicmu/qad_user_review) |

---

## Architecture Review Changes (v1.0.0 → v1.1.0)

### 1. New `AppSetting` Table — Dynamic Configuration

**Problem:** The "Database was last updated on Nov 24th, 2025" text was hardcoded in `ReviewList.cshtml`. Any data refresh required editing view source code.

**Solution:** Created `AppSetting` table (key-value store) in the database:
- `DatabaseLastUpdated` — date of last QAD data import
- `ReviewYear` / `ReviewQuarter` — current review period
- Values are read by `ReviewListController` and passed via `ReviewListMainViewModel.DatabaseLastUpdated`
- To update: simply `UPDATE AppSetting SET SettingValue = '2026-02-25' WHERE SettingKey = 'DatabaseLastUpdated'`

**Files changed:** `SQL_Server/AppSetting.sql`, `Models/AppSetting.cs`, `Data/UserReviewContext.cs`, `Controllers/ReviewListController.cs`, `ViewModels/ReviewList.cs`, `Views/ReviewList/ReviewList.cshtml`

### 2. Database Normalization — Single Source of Truth

**Problem:** Employee and manager information (name, email, plant, manager) was duplicated across `User`, `Manager`, and `ReviewList` tables. Changing a user's manager required updating 3+ tables.

**Solution:**
- `User` table remains the **single source of truth** for employee data
- `ReviewList` now has a FK relationship to `User` via `Username` → `User.Username`
- `ReviewList.User` navigation property allows EF Core to JOIN automatically
- `Manager` table now has a proper PK (`Id` identity) and unique constraint on `UserID`
- Proper indexes added: `IX_ReviewList_ManagerUsername`, `IX_ReviewList_Username`, `IX_User_ManagerUserName`
- Denormalized columns (`Employee`, `Email`, `Manager`, `Plant`) remain in `ReviewList` for backward compatibility; a future migration can drop them

**Files changed:** `SQL_Server/Migration_Normalize.sql`, `Models/User.cs`, `Models/Manager.cs`, `Models/ReviewList.cs`, `Data/UserReviewContext.cs`

### 3. Backend Naming Conventions

**Problem:** Mixed naming styles — `camelCase` properties in ViewModels (`selectedEmployee`), scaffolding artifacts (`Manager1`), dead code, wrong class names.

**Fixes applied:**
- ViewModel properties: `selectedEmployee` → `SelectedEmployee`, `selectedGroup` → `SelectedGroup` (PascalCase per C# convention)
- `Manager.Manager1` → `Manager.ManagerName` (meaningful name, still maps to DB column `Manager`)
- `Services/Class.cs` → `Services/EmailService.cs` (proper file naming)
- `ITQuality` → `ITQualityController` (class must have `Controller` suffix)
- `IHostingEnvironment` → `IWebHostEnvironment` (obsolete API replaced)
- `EmailService` refactored to implement `IEmailService` interface, registered in DI
- Deleted dead code: `DropdownEmployee.cs`, `Home/New.cshtml` action removed from `HomeController`
- Cleaned up unused `using` directives across all controllers

### 4. Frontend Naming Conventions & Code Quality

**Problem:** JavaScript had 3 near-identical 50+ line functions (`approveSelected`, `disableSelected`, `OpenSelected`), inconsistent naming, and `site.js` was loaded 3 times.

**Fixes applied:**
- Extracted `updateFilteredDecisions(fromValue, toValue)` and `matchesCurrentFilter()` — eliminated ~150 lines of duplicated code
- `OpenSelected()` → `openSelected()` (consistent camelCase for JS functions)
- Decision cell now uses `class="decision-cell"` instead of `id="DropdownDecision"` (IDs must be unique; there were many rows with the same ID)
- Removed duplicate `site.js` script references (was loaded 3× in `_Layout.cshtml`)
- Removed reference to non-existent `savechanges.js`

### 5. Frontend Design Fixes

**Problem:** Invalid HTML tags (`<h7>`, `<h8>`), inline styles everywhere, jQuery loaded after scripts that depend on it.

**Fixes applied:**
- Replaced `<h7>` / `<h8>` with `<p>` and `<p class="fw-bold">` (valid HTML)
- Moved inline styles to CSS classes: `.header-gradient`, `.panel-filter`, `.panel-actions`, `.panel-border-bottom`, `.status-resolved`, `.status-pending`, `.card-header-slim`, `.manual-icon`
- Fixed script loading order: jQuery → Bootstrap → site.js (was: site.js → jQuery → Bootstrap → site.js again)
- Used Bootstrap 5 `btn-close` class instead of `&times` character for modal close buttons
- Replaced `ml-1` (Bootstrap 4) with `ms-1` (Bootstrap 5)
- Replaced `text-left` (Bootstrap 4) with `text-start` (Bootstrap 5)

---

## Project Structure (v1.1.0)

```
QAD User Review 1.0.0/
├── Controllers/
│   ├── HomeController.cs           # Landing page
│   ├── SignInController.cs         # Sign-in form (incomplete)
│   ├── ReviewListController.cs     # Main controller — review CRUD, filtering, AppSetting
│   ├── ServiceDeskController.cs    # File listing/download from wwwroot/Files/ServiceDesk/
│   ├── ItDocsController.cs         # File listing/download from wwwroot/Files/
│   └── ITQualityController.cs      # File listing/download from wwwroot/ITQualityFiles/
├── Models/
│   ├── AppSetting.cs               # Key-value configuration (NEW)
│   ├── User.cs                     # Employee/user record (has PK, navigation property)
│   ├── Manager.cs                  # Authorized manager record (has PK now)
│   ├── ReviewList.cs               # Core review item with FK to User
│   └── MenuDetail.cs               # QAD menu/permission detail (HasNoKey)
├── ViewModels/
│   ├── ReviewList.cs               # ReviewListMainViewModel — filters + DatabaseLastUpdated
│   ├── SharedReviewList.cs         # SharedReviewListViewModel — partial view model
│   ├── MenuDetail.cs               # MenuDetailViewModel — modal popup data
│   └── FileModel.cs                # File name wrapper for download pages
├── Views/
│   ├── _ViewStart.cshtml
│   ├── _ViewImports.cshtml
│   ├── Home/
│   │   ├── Index.cshtml            # Welcome / landing page
│   │   ├── New.cshtml              # Test page (legacy)
│   │   └── Error.cshtml            # Error view
│   ├── SignIn/
│   │   └── SignIn.cshtml           # Sign-in form (incomplete)
│   ├── ReviewList/
│   │   ├── Index.cshtml            # Unused — redirects to ReviewList.cshtml
│   │   ├── ReviewList.cshtml       # Main review list with dynamic "last updated"
│   │   └── MenuDetails.cshtml      # Menu details partial
│   ├── ServiceDesk/
│   │   └── Index.cshtml            # Service Desk file listing
│   ├── ItDocs/
│   │   └── Index.cshtml            # IT Policies file listing
│   ├── ITQuality/
│   │   └── Index.cshtml            # IT Quality & Compliance file listing
│   └── Shared/
│       ├── _Layout.cshtml          # Main layout — fixed script order, proper Bootstrap 5
│       ├── _WorkingList.cshtml     # Review list table partial
│       ├── MenuDetails.cshtml      # Menu details modal partial (legacy test)
│       ├── _Popup.cshtml           # Loading spinner partial
│       └── _ValidationScriptsPartial.cshtml
├── Data/
│   └── UserReviewContext.cs        # EF Core DbContext (5 DbSets, FK relationships)
├── Services/
│   └── EmailService.cs             # IEmailService + EmailService — DI-registered
├── wwwroot/
│   ├── css/
│   │   ├── site.css                # Refactored — extracted inline styles to classes
│   │   ├── SignIn.css
│   │   └── Loader.css
│   ├── js/
│   │   └── site.js                 # Refactored — deduplicated, consistent naming
│   └── lib/                        # Bootstrap 5, jQuery, jQuery Validation
├── Program.cs                      # Entry point — registers IEmailService
├── appsettings.json
└── QAD User Review.csproj
```

---

## Database Schema (v1.1.0)

### Tables

| Table | Primary Key | Key Relationships |
|---|---|---|
| `ReviewList` | `ID` (int, identity) | FK → `User.Username`, FK → `Manager.UserID` |
| `User` | `Username` (varchar 20) | Referenced by ReviewList |
| `Manager` | `Id` (int, identity) | Unique on `UserID`, referenced by ReviewList |
| `MenuDetail` | None (HasNoKey) | Standalone lookup |
| `AppSetting` | `Id` (int, identity) | Unique on `SettingKey` |
| `ReviewList_History` | None | Archive table |
| `ApplicationUser` | None | Exists in DB, unused in app |

### Indexes

| Table | Index | Columns |
|---|---|---|
| ReviewList | `IX_ReviewList_ManagerUsername` | `ManagerUsername` |
| ReviewList | `IX_ReviewList_Username` | `Username` |
| User | `IX_User_ManagerUserName` | `ManagerUserName` |

---

## Technology Stack (v1.1.0)

| Concern | Current | Notes |
|---|---|---|
| Runtime | .NET 6.0 | End of support Nov 2024 — needs upgrade to .NET 8+ |
| Project type | ASP.NET Core MVC | Server-rendered Razor views |
| ORM | EF Core 6.0.9 (SqlServer) | FK relationships, navigation properties, indexes |
| Auth | Windows Authentication (IIS) | No middleware — relies on `User.Identity.Name` |
| Email | `IEmailService` (DI-registered) | SMTP via relay.spsx.com:25, config in appsettings |
| Frontend | Bootstrap 5, jQuery, jQuery Validation | Loaded from wwwroot/lib |
| Testing | None | |

---

## Remaining Issues / Future Work

### High Priority
- Upgrade from .NET 6.0 to .NET 8+ (6.0 is end-of-life)
- Move database credentials out of `appsettings.json` into User Secrets or environment variables
- Add `[Authorize]` attributes instead of manual `if` checks
- Replace hardcoded manager delegation mapping (`ResolveManagerUserId`) with a database-driven delegation table

### Medium Priority
- Add repository pattern / service layer to fully decouple controllers from DbContext
- Add FluentValidation for input validation
- Drop denormalized columns from ReviewList once the FK approach is proven in production
- Add proper error handling (global exception handler, ProblemDetails responses)

### Low Priority
- Add unit tests (Application layer) and integration tests (WebApplicationFactory)
- Consolidate file-browser controllers into a single generic controller
- Remove legacy pages (Home/New.cshtml, ReviewList/Index.cshtml redirect)
- Enable HTTPS redirection

---

## Coding Conventions

### Backend (C#)
- **PascalCase** for all public properties, methods, classes
- **camelCase** for local variables and private fields (prefixed with `_` for injected dependencies)
- Use `async`/`await` for all I/O operations
- Interfaces for all external dependencies (`IEmailService`, future `IReviewListService`, etc.)
- File-scoped namespaces preferred for new files

### Frontend (JavaScript)
- **camelCase** for all functions and variables (`approveSelected`, `filterTable`)
- **kebab-case** for CSS class names that are multi-word custom classes
- `"use strict"` at top of JS files
- No duplicate `id` attributes in HTML (use classes instead)
- jQuery loaded before any scripts that depend on it

### Database (SQL Server)
- **PascalCase** for table and column names
- All tables must have a primary key
- Foreign keys named `FK_{ChildTable}_{ParentTable}`
- Indexes named `IX_{Table}_{Column}`
- Unique constraints named `UQ_{Table}_{Column}`

---

## Notes for Claude

- The user is refactoring from a junior-level MVC codebase, so assume there may be anti-patterns.
- When reviewing or rewriting code, always explain *why* a change is being made, not just *what* the change is.
- Suggest tests alongside any new code written.
- Keep suggestions incremental and practical — avoid over-engineering.
- Default to modern C# idioms (record types, pattern matching, nullable reference types, file-scoped namespaces).
