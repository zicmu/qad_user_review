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

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
