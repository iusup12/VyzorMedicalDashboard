using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Vyzor.Domain.Entities;
using Vyzor.Infrastructure.Identity;

namespace Vyzor.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<DoctorSchedule> DoctorSchedules => Set<DoctorSchedule>();

    public DbSet<Specialization> Specializations => Set<Specialization>();

    public DbSet<Appointment> Appointments => Set<Appointment>();

    public DbSet<Review> Reviews => Set<Review>();

    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
    public DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureSpecialization(modelBuilder);
        ConfigureDoctor(modelBuilder);
        ConfigureDoctorSchedule(modelBuilder);
        ConfigurePatient(modelBuilder);

        ConfigureAppointment(modelBuilder);

        ConfigureReview(modelBuilder);

        ConfigureSubscriptionPlan(modelBuilder);
        ConfigureUserSubscription(modelBuilder);

        ConfigureAuditLog(modelBuilder);
    }

    private static void ConfigureSpecialization(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Specialization>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Description)
                .HasMaxLength(1000);

            entity.HasIndex(x => x.Name)
                .IsUnique();
        });
    }

    private static void ConfigureDoctor(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.FullName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.About)
                .HasMaxLength(3000);

            entity.Property(x => x.ImageUrl)
                .HasMaxLength(500);

            entity.Property(x => x.AppointmentPrice)
                .HasPrecision(18, 2);

            entity.HasIndex(x => x.SpecializationId);

            entity.HasOne(x => x.Specialization)
                .WithMany(x => x.Doctors)
                .HasForeignKey(x => x.SpecializationId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureDoctorSchedule(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DoctorSchedule>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.DoctorId);

            entity.HasOne(x => x.Doctor)
                .WithMany(x => x.Schedules)
                .HasForeignKey(x => x.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigurePatient(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.FullName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(x => x.Phone)
                .HasMaxLength(50);

            entity.Property(x => x.Gender)
                .HasMaxLength(20);

            entity.Property(x => x.Address)
                .HasMaxLength(500);

            entity.HasIndex(x => x.UserId);
            entity.HasIndex(x => x.Email);
        });
    }

    private static void ConfigureAppointment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.PatientId);
            entity.HasIndex(x => x.DoctorId);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.AppointmentDate);

            entity.Property(x => x.Price)
                .HasPrecision(18, 2);

            entity.Property(x => x.DiscountPercent)
                .HasPrecision(5, 2);

            entity.Property(x => x.FinalPrice)
                .HasPrecision(18, 2);

            entity.Property(x => x.About)
                .HasMaxLength(2000);

            entity.HasOne(x => x.Patient)
                .WithMany(x => x.Appointments)
                .HasForeignKey(x => x.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Doctor)
                .WithMany(x => x.Appointments)
                .HasForeignKey(x => x.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureReview(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.DoctorId);

            entity.Property(x => x.Comment)
                .HasMaxLength(2000);

            entity.Property(x => x.PatientId)
                .HasMaxLength(450);

            entity.HasOne(x => x.Doctor)
                .WithMany(x => x.Reviews)
                .HasForeignKey(x => x.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureSubscriptionPlan(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SubscriptionPlan>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.DiscountPercent)
                .HasPrecision(5, 2);

            entity.Property(x => x.MonthlyPrice)
                .HasPrecision(18, 2);
        });
    }

    private static void ConfigureUserSubscription(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserSubscription>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.UserId);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.EndDate);

            entity.HasOne(x => x.SubscriptionPlan)
                .WithMany()
                .HasForeignKey(x => x.SubscriptionPlanId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureAuditLog(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.UserId);
            entity.HasIndex(x => x.Action);
            entity.HasIndex(x => x.CreatedAt);

            entity.Property(x => x.EntityName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Description)
                .HasMaxLength(2000);

            entity.Property(x => x.OldValues)
                .HasMaxLength(4000);

            entity.Property(x => x.NewValues)
                .HasMaxLength(4000);
        });
    }
}