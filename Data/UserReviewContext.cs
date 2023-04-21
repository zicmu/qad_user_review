using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
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

      //  public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; } = null!;
        public virtual DbSet<Manager> Managers { get; set; } = null!;        
        public virtual DbSet<MenuDetail> MenuDetails { get; set; } = null!;        
        public virtual DbSet<ReviewList> ReviewLists { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }
       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          


        modelBuilder.Entity<Manager>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Manager");

                entity.Property(e => e.Email)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Manager1)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("Manager");

                entity.Property(e => e.UserId)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("UserID");
            });
          
            modelBuilder.Entity<MenuDetail>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Menu)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.UserGroupFullName)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });
           
          


            modelBuilder.Entity<ReviewList>(entity =>
            {
                entity.ToTable("ReviewList");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ChangedBy)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ChangedOn).HasColumnType("datetime");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Create_Date");

                entity.Property(e => e.Decision)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Employee)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Manager)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ManagerUsername)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Note)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Plant)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ReviewQuarter)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("Review_Quarter");

                entity.Property(e => e.ReviewYear).HasColumnName("Review_Year");

                entity.Property(e => e.System)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserGroup)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.UserGroupFullName)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });


        

           

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("User");

                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(e => e.Employee)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Plant)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Manager)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ManagerUserName)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Valid)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });




            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

