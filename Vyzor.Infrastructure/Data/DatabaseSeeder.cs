using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Vyzor.Domain.Entities;
using Vyzor.Domain.Enums;
using Vyzor.Infrastructure.Identity;

namespace Vyzor.Infrastructure.Data;

public class DatabaseSeeder
{
    public const string AdminEmail = "admin@vyzor.test";
    public const string DoctorEmail = "doctor@vyzor.test";
    public const string PatientEmail = "patient@vyzor.test";

    public const string DemoPassword = "Vyzor123!";

    private readonly AppDbContext _dbContext;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        AppDbContext dbContext,
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager,
        ILogger<DatabaseSeeder> logger)
    {
        _dbContext = dbContext;
        _roleManager = roleManager;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task SeedAsync(
        CancellationToken cancellationToken = default)
    {
        await SeedRolesAsync();

        var users = await SeedUsersAsync();

        var specializations =
            await SeedSpecializationsAsync(cancellationToken);

        var doctors =
            await SeedDoctorsAsync(
                specializations,
                cancellationToken);

        await SeedPatientsAsync(
            users.Patient,
            cancellationToken);

        await SeedDoctorSchedulesAsync(
            doctors,
            cancellationToken);

        var plans =
            await SeedSubscriptionPlansAsync(
                cancellationToken);

        await SeedUserSubscriptionsAsync(
            users.Patient.Id,
            plans.Base.Id,
            cancellationToken);

        await SeedAuditLogsAsync(
            users,
            cancellationToken);

        _logger.LogInformation(
            "Database seed completed.");
    }

    private async Task SeedRolesAsync()
    {
        foreach (var role in new[]
        {
            "Admin",
            "Doctor",
            "Patient"
        })
        {
            if (await _roleManager.RoleExistsAsync(role))
            {
                continue;
            }

            await _roleManager.CreateAsync(
                new IdentityRole(role));
        }
    }

    private async Task<(
        ApplicationUser Admin,
        ApplicationUser Doctor,
        ApplicationUser Patient)>
        SeedUsersAsync()
    {
        var admin =
            await EnsureUserAsync(
                AdminEmail,
                "System Administrator",
                "Admin");

        var doctor =
            await EnsureUserAsync(
                DoctorEmail,
                "Dr. John Smith",
                "Doctor");

        var patient =
            await EnsureUserAsync(
                PatientEmail,
                "Demo Patient",
                "Patient");

        return (admin, doctor, patient);
    }

    private async Task<ApplicationUser> EnsureUserAsync(
        string email,
        string fullName,
        string role)
    {
        var user =
            await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FullName = fullName
            };

            var result =
                await _userManager.CreateAsync(
                    user,
                    DemoPassword);

            EnsureSuccess(result);
        }

        if (!await _userManager.IsInRoleAsync(user, role))
        {
            var result =
                await _userManager.AddToRoleAsync(
                    user,
                    role);

            EnsureSuccess(result);
        }

        return user;
    }

    private async Task<List<Specialization>>
        SeedSpecializationsAsync(
            CancellationToken cancellationToken)
    {
        if (await _dbContext.Specializations.AnyAsync(
                cancellationToken))
        {
            return await _dbContext.Specializations
                .ToListAsync(cancellationToken);
        }

        var specializations = new List<Specialization>
        {
            new()
            {
                Name = "Therapist",
                Description = "General practitioner"
            },

            new()
            {
                Name = "Cardiologist",
                Description = "Heart specialist"
            },

            new()
            {
                Name = "Dentist",
                Description = "Dental specialist"
            },

            new()
            {
                Name = "Neurologist",
                Description = "Nervous system specialist"
            },

            new()
            {
                Name = "Pediatrician",
                Description = "Children specialist"
            }
        };

        _dbContext.Specializations.AddRange(
            specializations);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return specializations;
    }

    private async Task<List<Doctor>>
        SeedDoctorsAsync(
            List<Specialization> specializations,
            CancellationToken cancellationToken)
    {
        var john =
            await _dbContext.Doctors
                .FirstOrDefaultAsync(
                    x => x.FullName == "Dr. John Smith",
                    cancellationToken);

        if (john == null)
        {
            john = new Doctor
            {
                FullName = "Dr. John Smith",
                About = "Experienced therapist.",
                ExperienceYears = 12,
                AppointmentPrice = 100,
                ImageUrl =
                    "/assets/doccure/img/doctor-grid/doctor-grid-01.jpg",
                SpecializationId = specializations[0].Id,
                SpecializationName = specializations[0].Name,
                IsActive = true
            };

            _dbContext.Doctors.Add(john);
        }
        else
        {
            john.ImageUrl =
                "/assets/doccure/img/doctor-grid/doctor-grid-01.jpg";

            john.SpecializationId =
                specializations[0].Id;

            john.SpecializationName =
                specializations[0].Name;

            john.IsActive = true;
        }

        var sarah =
            await _dbContext.Doctors
                .FirstOrDefaultAsync(
                    x => x.FullName == "Dr. Sarah Wilson",
                    cancellationToken);

        if (sarah == null)
        {
            sarah = new Doctor
            {
                FullName = "Dr. Sarah Wilson",
                About = "Cardiology specialist.",
                ExperienceYears = 8,
                AppointmentPrice = 120,
                ImageUrl =
                    "/assets/doccure/img/doctor-grid/doctor-grid-02.jpg",
                SpecializationId = specializations[1].Id,
                SpecializationName = specializations[1].Name,
                IsActive = true
            };

            _dbContext.Doctors.Add(sarah);
        }
        else
        {
            sarah.ImageUrl =
                "/assets/doccure/img/doctor-grid/doctor-grid-02.jpg";

            sarah.SpecializationId =
                specializations[1].Id;

            sarah.SpecializationName =
                specializations[1].Name;

            sarah.IsActive = true;
        }

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return await _dbContext.Doctors
            .ToListAsync(cancellationToken);
    }

    private async Task SeedPatientsAsync(
        ApplicationUser user,
        CancellationToken cancellationToken)
    {
        if (await _dbContext.Patients.AnyAsync(
                cancellationToken))
        {
            return;
        }

        _dbContext.Patients.Add(
            new Patient
            {
                UserId = user.Id,
                FullName = "Demo Patient",
                Email = user.Email!,
                Phone = "+10000000000",
                DateOfBirth = new DateTime(
    1995, 5, 15,
    0, 0, 0,
    DateTimeKind.Utc),
                Gender = "Male",
                Address = "Demo Address"
            });

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    private async Task SeedDoctorSchedulesAsync(
        List<Doctor> doctors,
        CancellationToken cancellationToken)
    {
        if (await _dbContext.DoctorSchedules.AnyAsync(
                cancellationToken))
        {
            return;
        }

        foreach (var doctor in doctors)
        {
            for (var i = 1; i <= 7; i++)
            {
                _dbContext.DoctorSchedules.Add(
                    new DoctorSchedule
                    {
                        DoctorId = doctor.Id,
                        Date = DateOnly.FromDateTime(
                            DateTime.Today.AddDays(i)),
                        StartTime = new TimeOnly(9, 0),
                        EndTime = new TimeOnly(17, 0),
                        IsAvailable = true
                    });
            }
        }

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    private async Task<(
        SubscriptionPlan Base,
        SubscriptionPlan Premium)>
        SeedSubscriptionPlansAsync(
            CancellationToken cancellationToken)
    {
        if (!await _dbContext.SubscriptionPlans
                .AnyAsync(cancellationToken))
        {
            _dbContext.SubscriptionPlans.AddRange(
                new SubscriptionPlan
                {
                    Name = "Base",
                    DiscountPercent = 7,
                    MonthlyPrice = 9.99m
                },

                new SubscriptionPlan
                {
                    Name = "Premium",
                    DiscountPercent = 10,
                    MonthlyPrice = 19.99m
                });

            await _dbContext.SaveChangesAsync(
                cancellationToken);
        }

        var plans =
            await _dbContext.SubscriptionPlans
                .ToListAsync(cancellationToken);

        return (
            plans.First(x => x.Name == "Base"),
            plans.First(x => x.Name == "Premium"));
    }

    private async Task SeedUserSubscriptionsAsync(
        string userId,
        int planId,
        CancellationToken cancellationToken)
    {
        if (await _dbContext.UserSubscriptions
                .AnyAsync(cancellationToken))
        {
            return;
        }

        _dbContext.UserSubscriptions.Add(
            new UserSubscription
            {
                UserId = userId,
                SubscriptionPlanId = planId,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1),
                Status = SubscriptionStatus.Active
            });

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    private async Task SeedAuditLogsAsync(
        (
            ApplicationUser Admin,
            ApplicationUser Doctor,
            ApplicationUser Patient) users,
        CancellationToken cancellationToken)
    {
        if (await _dbContext.AuditLogs
                .AnyAsync(cancellationToken))
        {
            return;
        }

        _dbContext.AuditLogs.Add(
            new AuditLog
            {
                UserId = users.Admin.Id,
                Action = AuditAction.Register,
                EntityName = "DatabaseSeeder",
                Description = "Initial seed completed",
                CreatedAt = DateTime.UtcNow
            });

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    private static void EnsureSuccess(
        IdentityResult result)
    {
        if (result.Succeeded)
        {
            return;
        }

        throw new InvalidOperationException(
            string.Join(
                "; ",
                result.Errors.Select(
                    x => x.Description)));
    }
}