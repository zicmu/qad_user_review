using Microsoft.EntityFrameworkCore;
using QAD_User_Review.Models;

namespace QAD_User_Review.Data
{
    public partial class UserReviewContext : DbContext
    {
        public UserReviewContext()
        {
        }

        public UserReviewContext(DbContextOptions<UserReviewContext> options)
            : base(options)
        {
        }

        public virtual DbSet<DimEmployee> DimEmployees { get; set; } = null!;
        public virtual DbSet<DimSystemIdentity> DimSystemIdentities { get; set; } = null!;
        public virtual DbSet<DimRole> DimRoles { get; set; } = null!;
        public virtual DbSet<DimMenu> DimMenus { get; set; } = null!;
        public virtual DbSet<RefReviewStatus> RefReviewStatuses { get; set; } = null!;
        public virtual DbSet<DimReviewPeriod> DimReviewPeriods { get; set; } = null!;
        public virtual DbSet<BridgeOrgChart> BridgeOrgCharts { get; set; } = null!;
        public virtual DbSet<FactUserRoleAssignment> FactUserRoleAssignments { get; set; } = null!;
        public virtual DbSet<FactRoleReview> FactRoleReviews { get; set; } = null!;
        public virtual DbSet<RefAppRole> RefAppRoles { get; set; } = null!;
        public virtual DbSet<RefAppFeature> RefAppFeatures { get; set; } = null!;
        public virtual DbSet<BridgeRoleFeature> BridgeRoleFeatures { get; set; } = null!;
        public virtual DbSet<AppUser> AppUsers { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DimEmployee>(entity =>
            {
                entity.ToTable("Dim_Employee");
                entity.HasKey(e => e.EmployeeKey);

                entity.Property(e => e.AD_Username)
                    .HasMaxLength(50).IsUnicode(false).IsRequired();
                entity.HasIndex(e => e.AD_Username).IsUnique();

                entity.Property(e => e.FullName)
                    .HasMaxLength(100).IsUnicode(false).IsRequired();
                entity.Property(e => e.Email)
                    .HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.Country)
                    .HasMaxLength(50).IsUnicode(false);
                entity.Property(e => e.CreatedAt).HasColumnType("datetime2");
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime2");
            });

            modelBuilder.Entity<DimSystemIdentity>(entity =>
            {
                entity.ToTable("Dim_SystemIdentity");
                entity.HasKey(e => e.SystemIdentityKey);

                entity.Property(e => e.SystemUsername)
                    .HasMaxLength(50).IsUnicode(false).IsRequired();
                entity.Property(e => e.SourceSystem)
                    .HasMaxLength(10).IsUnicode(false).IsRequired();
                entity.Property(e => e.Plant)
                    .HasMaxLength(50).IsUnicode(false);
                entity.Property(e => e.UserType)
                    .HasMaxLength(50).IsUnicode(false);
                entity.Property(e => e.ETL_LoadDate).HasColumnType("datetime2");

                entity.HasIndex(e => new { e.SystemUsername, e.SourceSystem })
                    .IsUnique()
                    .HasDatabaseName("UQ_SystemIdentity");

                entity.HasOne(e => e.Employee)
                    .WithMany(d => d.SystemIdentities)
                    .HasForeignKey(e => e.EmployeeKey)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DimRole>(entity =>
            {
                entity.ToTable("Dim_Role");
                entity.HasKey(e => e.RoleKey);

                entity.Property(e => e.RoleCode)
                    .HasMaxLength(50).IsUnicode(false).IsRequired();
                entity.Property(e => e.RoleDescription)
                    .HasMaxLength(200).IsUnicode(false);
                entity.Property(e => e.SourceSystem)
                    .HasMaxLength(10).IsUnicode(false).IsRequired();

                entity.HasIndex(e => new { e.RoleCode, e.SourceSystem })
                    .IsUnique()
                    .HasDatabaseName("UQ_Role");
            });

            modelBuilder.Entity<DimMenu>(entity =>
            {
                entity.ToTable("Dim_Menu");
                entity.HasKey(e => e.MenuKey);

                entity.Property(e => e.MenuCode)
                    .HasMaxLength(50).IsUnicode(false).IsRequired();
                entity.Property(e => e.MenuDescription)
                    .HasMaxLength(200).IsUnicode(false);

                entity.HasOne(e => e.Role)
                    .WithMany(r => r.Menus)
                    .HasForeignKey(e => e.RoleKey)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<RefReviewStatus>(entity =>
            {
                entity.ToTable("Ref_ReviewStatus");
                entity.HasKey(e => e.StatusKey);

                entity.Property(e => e.StatusCode)
                    .HasMaxLength(20).IsUnicode(false).IsRequired();
                entity.HasIndex(e => e.StatusCode).IsUnique();

                entity.Property(e => e.StatusDescription)
                    .HasMaxLength(100).IsUnicode(false);
            });

            modelBuilder.Entity<DimReviewPeriod>(entity =>
            {
                entity.ToTable("Dim_ReviewPeriod");
                entity.HasKey(e => e.ReviewPeriodKey);

                entity.Property(e => e.PeriodName)
                    .HasMaxLength(50).IsUnicode(false).IsRequired();
                entity.Property(e => e.PeriodType)
                    .HasMaxLength(20).IsUnicode(false).IsRequired();
                entity.Property(e => e.StartDate).HasColumnType("date");
                entity.Property(e => e.EndDate).HasColumnType("date");
            });

            modelBuilder.Entity<BridgeOrgChart>(entity =>
            {
                entity.ToTable("Bridge_OrgChart");
                entity.HasKey(e => e.OrgChartKey);

                entity.Property(e => e.SourceSystem)
                    .HasMaxLength(10).IsUnicode(false).IsRequired();
                entity.Property(e => e.ValidFrom).HasColumnType("date");
                entity.Property(e => e.ValidTo).HasColumnType("date");
                entity.Property(e => e.ETL_LoadDate).HasColumnType("datetime2");

                entity.HasOne(e => e.Employee)
                    .WithMany(d => d.ManagedEmployees)
                    .HasForeignKey(e => e.EmployeeKey)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Reviewer)
                    .WithMany(d => d.ReviewerFor)
                    .HasForeignKey(e => e.ReviewerKey)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<FactUserRoleAssignment>(entity =>
            {
                entity.ToTable("Fact_UserRoleAssignment");
                entity.HasKey(e => e.AssignmentKey);

                entity.Property(e => e.SourceSystem)
                    .HasMaxLength(10).IsUnicode(false).IsRequired();
                entity.Property(e => e.ETL_LoadDate).HasColumnType("datetime2");

                entity.HasIndex(e => new { e.SystemIdentityKey, e.RoleKey })
                    .IsUnique()
                    .HasDatabaseName("UQ_Assignment");

                entity.HasOne(e => e.SystemIdentity)
                    .WithMany(s => s.RoleAssignments)
                    .HasForeignKey(e => e.SystemIdentityKey)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Role)
                    .WithMany(r => r.RoleAssignments)
                    .HasForeignKey(e => e.RoleKey)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<FactRoleReview>(entity =>
            {
                entity.ToTable("Fact_RoleReview");
                entity.HasKey(e => e.ReviewKey);

                entity.Property(e => e.ReviewerComment)
                    .HasMaxLength(500).IsUnicode(false);
                entity.Property(e => e.ReviewedAt).HasColumnType("datetime2");
                entity.Property(e => e.LastModifiedAt).HasColumnType("datetime2");
                entity.Property(e => e.CreatedAt).HasColumnType("datetime2");
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime2");

                entity.HasOne(e => e.ReviewPeriod)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(e => e.ReviewPeriodKey)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Assignment)
                    .WithMany(a => a.Reviews)
                    .HasForeignKey(e => e.AssignmentKey)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Reviewer)
                    .WithMany(d => d.Reviews)
                    .HasForeignKey(e => e.ReviewerKey)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Status)
                    .WithMany(s => s.Reviews)
                    .HasForeignKey(e => e.StatusKey)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== RBAC Tables =====

            modelBuilder.Entity<RefAppRole>(entity =>
            {
                entity.ToTable("Ref_AppRole");
                entity.HasKey(e => e.AppRoleKey);

                entity.Property(e => e.RoleCode)
                    .HasMaxLength(20).IsUnicode(false).IsRequired();
                entity.HasIndex(e => e.RoleCode).IsUnique();

                entity.Property(e => e.RoleDescription)
                    .HasMaxLength(100).IsUnicode(false);

                entity.HasData(
                    new RefAppRole { AppRoleKey = 1, RoleCode = "SuperAdmin", RoleDescription = "Full access including role management" },
                    new RefAppRole { AppRoleKey = 2, RoleCode = "Admin", RoleDescription = "Manages users, imports, and employee-reviewer assignments" },
                    new RefAppRole { AppRoleKey = 3, RoleCode = "Auditor", RoleDescription = "Read-only unfiltered access to all reviews" },
                    new RefAppRole { AppRoleKey = 4, RoleCode = "Reviewer", RoleDescription = "Can review and submit decisions for own employees only" }
                );
            });

            modelBuilder.Entity<RefAppFeature>(entity =>
            {
                entity.ToTable("Ref_AppFeature");
                entity.HasKey(e => e.FeatureKey);

                entity.Property(e => e.FeatureCode)
                    .HasMaxLength(50).IsUnicode(false).IsRequired();
                entity.HasIndex(e => e.FeatureCode).IsUnique();

                entity.Property(e => e.FeatureDescription)
                    .HasMaxLength(100).IsUnicode(false);

                entity.HasData(
                    new RefAppFeature { FeatureKey = 1, FeatureCode = "ReviewAll", FeatureDescription = "View reviews for all employees unfiltered" },
                    new RefAppFeature { FeatureKey = 2, FeatureCode = "ReviewOwn", FeatureDescription = "View reviews scoped to own employees" },
                    new RefAppFeature { FeatureKey = 3, FeatureCode = "SubmitDecision", FeatureDescription = "Submit Approved/Disabled decision + comment" },
                    new RefAppFeature { FeatureKey = 4, FeatureCode = "ManageEmployeeReviewer", FeatureDescription = "View and edit employee-reviewer assignments" },
                    new RefAppFeature { FeatureKey = 5, FeatureCode = "Import", FeatureDescription = "Upload and process Excel files" },
                    new RefAppFeature { FeatureKey = 6, FeatureCode = "Reports", FeatureDescription = "View audit history and reporting" },
                    new RefAppFeature { FeatureKey = 7, FeatureCode = "ManageUsers", FeatureDescription = "Add/edit/deactivate app users and their roles" },
                    new RefAppFeature { FeatureKey = 8, FeatureCode = "ManageRoles", FeatureDescription = "Edit role-feature permission assignments" },
                    new RefAppFeature { FeatureKey = 9, FeatureCode = "ManagePeriod", FeatureDescription = "Open and close review periods, trigger archive" }
                );
            });

            modelBuilder.Entity<BridgeRoleFeature>(entity =>
            {
                entity.ToTable("Bridge_RoleFeature");
                entity.HasKey(e => e.RoleFeatureKey);

                entity.HasIndex(e => new { e.AppRoleKey, e.FeatureKey })
                    .IsUnique()
                    .HasDatabaseName("UQ_RoleFeature");

                entity.HasOne(e => e.AppRole)
                    .WithMany(r => r.RoleFeatures)
                    .HasForeignKey(e => e.AppRoleKey)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Feature)
                    .WithMany(f => f.RoleFeatures)
                    .HasForeignKey(e => e.FeatureKey)
                    .OnDelete(DeleteBehavior.Restrict);

                // Seed: permission matrix from architecture doc
                // SuperAdmin (1): all features
                // Admin (2): all except ManageRoles
                // Auditor (3): ReviewAll, ReviewOwn, Reports
                // Reviewer (4): ReviewOwn, SubmitDecision
                entity.HasData(
                    // SuperAdmin - all 9 features
                    new BridgeRoleFeature { RoleFeatureKey = 1,  AppRoleKey = 1, FeatureKey = 1, CanAccess = true },
                    new BridgeRoleFeature { RoleFeatureKey = 2,  AppRoleKey = 1, FeatureKey = 2, CanAccess = true },
                    new BridgeRoleFeature { RoleFeatureKey = 3,  AppRoleKey = 1, FeatureKey = 3, CanAccess = true },
                    new BridgeRoleFeature { RoleFeatureKey = 4,  AppRoleKey = 1, FeatureKey = 4, CanAccess = true },
                    new BridgeRoleFeature { RoleFeatureKey = 5,  AppRoleKey = 1, FeatureKey = 5, CanAccess = true },
                    new BridgeRoleFeature { RoleFeatureKey = 6,  AppRoleKey = 1, FeatureKey = 6, CanAccess = true },
                    new BridgeRoleFeature { RoleFeatureKey = 7,  AppRoleKey = 1, FeatureKey = 7, CanAccess = true },
                    new BridgeRoleFeature { RoleFeatureKey = 8,  AppRoleKey = 1, FeatureKey = 8, CanAccess = true },
                    new BridgeRoleFeature { RoleFeatureKey = 9,  AppRoleKey = 1, FeatureKey = 9, CanAccess = true },
                    // Admin - all except ManageRoles (8)
                    new BridgeRoleFeature { RoleFeatureKey = 10, AppRoleKey = 2, FeatureKey = 1, CanAccess = true },
                    new BridgeRoleFeature { RoleFeatureKey = 11, AppRoleKey = 2, FeatureKey = 2, CanAccess = true },
                    new BridgeRoleFeature { RoleFeatureKey = 12, AppRoleKey = 2, FeatureKey = 3, CanAccess = true },
                    new BridgeRoleFeature { RoleFeatureKey = 13, AppRoleKey = 2, FeatureKey = 4, CanAccess = true },
                    new BridgeRoleFeature { RoleFeatureKey = 14, AppRoleKey = 2, FeatureKey = 5, CanAccess = true },
                    new BridgeRoleFeature { RoleFeatureKey = 15, AppRoleKey = 2, FeatureKey = 6, CanAccess = true },
                    new BridgeRoleFeature { RoleFeatureKey = 16, AppRoleKey = 2, FeatureKey = 7, CanAccess = true },
                    new BridgeRoleFeature { RoleFeatureKey = 17, AppRoleKey = 2, FeatureKey = 8, CanAccess = false },
                    new BridgeRoleFeature { RoleFeatureKey = 18, AppRoleKey = 2, FeatureKey = 9, CanAccess = true },
                    // Auditor - ReviewAll, ReviewOwn, Reports only
                    new BridgeRoleFeature { RoleFeatureKey = 19, AppRoleKey = 3, FeatureKey = 1, CanAccess = true },
                    new BridgeRoleFeature { RoleFeatureKey = 20, AppRoleKey = 3, FeatureKey = 2, CanAccess = true },
                    new BridgeRoleFeature { RoleFeatureKey = 21, AppRoleKey = 3, FeatureKey = 3, CanAccess = false },
                    new BridgeRoleFeature { RoleFeatureKey = 22, AppRoleKey = 3, FeatureKey = 4, CanAccess = false },
                    new BridgeRoleFeature { RoleFeatureKey = 23, AppRoleKey = 3, FeatureKey = 5, CanAccess = false },
                    new BridgeRoleFeature { RoleFeatureKey = 24, AppRoleKey = 3, FeatureKey = 6, CanAccess = true },
                    new BridgeRoleFeature { RoleFeatureKey = 25, AppRoleKey = 3, FeatureKey = 7, CanAccess = false },
                    new BridgeRoleFeature { RoleFeatureKey = 26, AppRoleKey = 3, FeatureKey = 8, CanAccess = false },
                    new BridgeRoleFeature { RoleFeatureKey = 27, AppRoleKey = 3, FeatureKey = 9, CanAccess = false },
                    // Reviewer - ReviewOwn, SubmitDecision only
                    new BridgeRoleFeature { RoleFeatureKey = 28, AppRoleKey = 4, FeatureKey = 1, CanAccess = false },
                    new BridgeRoleFeature { RoleFeatureKey = 29, AppRoleKey = 4, FeatureKey = 2, CanAccess = true },
                    new BridgeRoleFeature { RoleFeatureKey = 30, AppRoleKey = 4, FeatureKey = 3, CanAccess = true },
                    new BridgeRoleFeature { RoleFeatureKey = 31, AppRoleKey = 4, FeatureKey = 4, CanAccess = false },
                    new BridgeRoleFeature { RoleFeatureKey = 32, AppRoleKey = 4, FeatureKey = 5, CanAccess = false },
                    new BridgeRoleFeature { RoleFeatureKey = 33, AppRoleKey = 4, FeatureKey = 6, CanAccess = false },
                    new BridgeRoleFeature { RoleFeatureKey = 34, AppRoleKey = 4, FeatureKey = 7, CanAccess = false },
                    new BridgeRoleFeature { RoleFeatureKey = 35, AppRoleKey = 4, FeatureKey = 8, CanAccess = false },
                    new BridgeRoleFeature { RoleFeatureKey = 36, AppRoleKey = 4, FeatureKey = 9, CanAccess = false }
                );
            });

            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.ToTable("App_User");
                entity.HasKey(e => e.AppUserKey);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime2");
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime2");

                entity.HasOne(e => e.Employee)
                    .WithMany(d => d.AppUserAccounts)
                    .HasForeignKey(e => e.EmployeeKey)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.AppRole)
                    .WithMany(r => r.AppUsers)
                    .HasForeignKey(e => e.AppRoleKey)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.AssignedByEmployee)
                    .WithMany(d => d.AppUserAssignments)
                    .HasForeignKey(e => e.AssignedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
