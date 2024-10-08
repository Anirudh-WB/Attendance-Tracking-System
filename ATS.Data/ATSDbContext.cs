﻿using ATS.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATS.Data
{
    public class ATSDbContext : DbContext
    {
        public ATSDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GetTotalHours>(entity =>
            {
                entity.HasNoKey(); // Indicate that this entity does not have a key
                entity.ToView(null); // Optional: specify that this entity does not map to a database view
            });

            modelBuilder.Entity<GetTotalOutHours>(entity =>
            {
                entity.HasNoKey(); // Indicate that this entity does not have a key
                entity.ToView(null); // Optional: specify that this entity does not map to a database view
            });

            modelBuilder.Entity<GetTotalInHours>(entity =>
            {
                entity.HasNoKey(); // Indicate that this entity does not have a key
                entity.ToView(null); // Optional: specify that this entity does not map to a database view
            });

            modelBuilder.Entity<AttendanceLogWithDetails>(entity =>
            {
                entity.HasNoKey(); // Indicate that this entity does not have a key
                entity.ToView(null); // Optional: specify that this entity does not map to a database view
            });

            modelBuilder.Entity<MisEntrySummary>(entity =>
            {
                entity.HasNoKey(); // Indicate that this entity does not have a key
                entity.ToView(null); // Optional: specify that this entity does not map to a database view
            });

            modelBuilder.Entity<GetStatusOfAttendanceLog>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("Id");
                entity.Property(e => e.UserId)
                    .HasColumnName("UserId");
            });
                
            modelBuilder.Entity<GetSumTotalHours>(entity =>
            {
                entity.HasNoKey(); // Indicate that this entity does not have a key
                entity.ToView(null); // Optional: specify that this entity does not map to a database view
            });

            modelBuilder.Entity<User>()
            .HasOne(u => u.EmployeeDetail)
            .WithOne(e => e.User)
            .HasForeignKey<EmployeeDetail>(e => e.UserId);
        }

        public DbSet<EmployeeDetail> employeeDetails { get; set; }
        public DbSet<AttendanceLog> attendanceLogs { get; set; }
        public DbSet<Designation> designations { get; set; }
        public DbSet<Gender> genders { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<Role> roles { get; set; }
        public DbSet<Page> pages { get; set; }
        public DbSet<AccessPage> accessPages { get; set; }
    }
}
