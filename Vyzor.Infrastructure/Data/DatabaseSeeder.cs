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

    public int SubscriptionPlanId { get; private set; }

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
        var doctors = new List<Doctor>
    {
        new()
        {
            FullName = "Dr. John Smith",
            About = "Experienced therapist with extensive clinical practice.",
            ExperienceYears = 12,
            Rating = 5,
            ImageUrl = "/assets/doccure/img/doctor-grid/doctor-grid-01.jpg",
            SpecializationId = specializations.First(x => x.Name == "Therapist").Id,
            SpecializationName = "Therapist",
            Education = "Harvard Medical School",
            AppointmentPrice = 100m,
            Clinic = "Vyzor Medical Center",
            Email = "john.smith@vyzor.test",
            PhoneNumber = "+10000000001",
            IsActive = true
        },

        new()
        {
            FullName = "Dr. Sarah Wilson",
            About = "Experienced cardiologist specializing in cardiovascular diseases.",
            ExperienceYears = 8,
            Rating = 5,
            ImageUrl = "/assets/doccure/img/doctor-grid/doctor-grid-02.jpg",
            SpecializationId = specializations.First(x => x.Name == "Cardiologist").Id,
            SpecializationName = "Cardiologist",
            Education = "Johns Hopkins University",
            AppointmentPrice = 120m,
            Clinic = "Vyzor Heart Clinic",
            Email = "sarah.wilson@vyzor.test",
            PhoneNumber = "+10000000002",
            IsActive = true
        },

        new()
        {
            FullName = "Dr. Michael Brown",
            About = "Professional dentist focused on preventive and restorative dentistry.",
            ExperienceYears = 10,
            Rating = 5,
            ImageUrl = "/assets/doccure/img/doctor-grid/doctor-grid-03.jpg",
            SpecializationId = specializations.First(x => x.Name == "Dentist").Id,
            SpecializationName = "Dentist",
            Education = "University of Pennsylvania",
            AppointmentPrice = 90m,
            Clinic = "Vyzor Dental Clinic",
            Email = "michael.brown@vyzor.test",
            PhoneNumber = "+10000000003",
            IsActive = true
        },

        new()
        {
            FullName = "Dr. Emily Davis",
            About = "Neurologist specializing in diagnosis and treatment of neurological disorders.",
            ExperienceYears = 9,
            Rating = 5,
            ImageUrl = "/assets/doccure/img/doctor-grid/doctor-grid-04.jpg",
            SpecializationId = specializations.First(x => x.Name == "Neurologist").Id,
            SpecializationName = "Neurologist",
            Education = "Stanford University School of Medicine",
            AppointmentPrice = 130m,
            Clinic = "Vyzor Neurology Center",
            Email = "emily.davis@vyzor.test",
            PhoneNumber = "+10000000004",
            IsActive = true
        },

        new()
        {
            FullName = "Dr. James Anderson",
            About = "Pediatrician providing comprehensive healthcare for children.",
            ExperienceYears = 11,
            Rating = 5,
            ImageUrl = "/assets/doccure/img/doctor-grid/doctor-grid-05.jpg",
            SpecializationId = specializations.First(x => x.Name == "Pediatrician").Id,
            SpecializationName = "Pediatrician",
            Education = "Yale School of Medicine",
            AppointmentPrice = 110m,
            Clinic = "Vyzor Children's Clinic",
            Email = "james.anderson@vyzor.test",
            PhoneNumber = "+10000000005",
            IsActive = true
        },

        new()
        {
            FullName = "Dr. Olivia Martinez",
            About = "Therapist focused on general medicine and preventive healthcare.",
            ExperienceYears = 7,
            Rating = 4,
            ImageUrl = "/assets/doccure/img/doctor-grid/doctor-grid-06.jpg",
            SpecializationId = specializations.First(x => x.Name == "Therapist").Id,
            SpecializationName = "Therapist",
            Education = "University of California Medical School",
            AppointmentPrice = 85m,
            Clinic = "Vyzor Medical Center",
            Email = "olivia.martinez@vyzor.test",
            PhoneNumber = "+10000000006",
            IsActive = true
        },

        new()
        {
            FullName = "Dr. Daniel Taylor",
            About = "Senior cardiologist with extensive experience in cardiovascular medicine.",
            ExperienceYears = 14,
            Rating = 5,
            ImageUrl = "/assets/doccure/img/doctor-grid/doctor-grid-07.jpg",
            SpecializationId = specializations.First(x => x.Name == "Cardiologist").Id,
            SpecializationName = "Cardiologist",
            Education = "Columbia University Vagelos College",
            AppointmentPrice = 150m,
            Clinic = "Vyzor Heart Clinic",
            Email = "daniel.taylor@vyzor.test",
            PhoneNumber = "+10000000007",
            IsActive = true
        },

        new()
        {
            FullName = "Dr. Sophia Thomas",
            About = "Dentist specializing in cosmetic and general dental care.",
            ExperienceYears = 6,
            Rating = 4,
            ImageUrl = "/assets/doccure/img/doctor-grid/doctor-grid-08.jpg",
            SpecializationId = specializations.First(x => x.Name == "Dentist").Id,
            SpecializationName = "Dentist",
            Education = "New York University College of Dentistry",
            AppointmentPrice = 95m,
            Clinic = "Vyzor Dental Clinic",
            Email = "sophia.thomas@vyzor.test",
            PhoneNumber = "+10000000008",
            IsActive = true
        },

        new()
        {
            FullName = "Dr. William Jackson",
            About = "Senior neurologist specializing in complex neurological conditions.",
            ExperienceYears = 15,
            Rating = 5,
            ImageUrl = "/assets/doccure/img/doctor-grid/doctor-grid-09.jpg",
            SpecializationId = specializations.First(x => x.Name == "Neurologist").Id,
            SpecializationName = "Neurologist",
            Education = "Mayo Clinic Alix School of Medicine",
            AppointmentPrice = 160m,
            Clinic = "Vyzor Neurology Center",
            Email = "william.jackson@vyzor.test",
            PhoneNumber = "+10000000009",
            IsActive = true
        },

        new()
        {
            FullName = "Dr. Isabella White",
            About = "Pediatrician providing friendly and comprehensive children's healthcare.",
            ExperienceYears = 5,
            Rating = 4,
            ImageUrl = "/assets/doccure/img/doctor-grid/doctor-grid-10.jpg",
            SpecializationId = specializations.First(x => x.Name == "Pediatrician").Id,
            SpecializationName = "Pediatrician",
            Education = "Boston University School of Medicine",
            AppointmentPrice = 100m,
            Clinic = "Vyzor Children's Clinic",
            Email = "isabella.white@vyzor.test",
            PhoneNumber = "+10000000010",
            IsActive = true
        },
        
new()
{
    FullName = "Dr. Robert Clark",
    About = "Experienced therapist specializing in preventive care and general medicine.",
    ExperienceYears = 9,
    Rating = 4,
    ImageUrl = "/assets/doccure/img/doctor-grid/doctor-grid-11.jpg",
    SpecializationId = specializations.First(x => x.Name == "Therapist").Id,
    SpecializationName = "Therapist",
    Education = "University of Michigan Medical School",
    AppointmentPrice = 95m,
    Clinic = "Vyzor Medical Center",
    Email = "robert.clark@vyzor.test",
    PhoneNumber = "+10000000011",
    IsActive = true
},

new()
{
    FullName = "Dr. Emma Johnson",
    About = "Cardiologist specializing in heart disease prevention and cardiovascular treatment.",
    ExperienceYears = 10,
    Rating = 5,
    ImageUrl = "/assets/doccure/img/doctor-grid/doctor-grid-12.jpg",
    SpecializationId = specializations.First(x => x.Name == "Cardiologist").Id,
    SpecializationName = "Cardiologist",
    Education = "Duke University School of Medicine",
    AppointmentPrice = 140m,
    Clinic = "Vyzor Heart Clinic",
    Email = "emma.johnson@vyzor.test",
    PhoneNumber = "+10000000012",
    IsActive = true
},

new()
{
    FullName = "Dr. Christopher Miller",
    About = "Professional dentist specializing in restorative and cosmetic dentistry.",
    ExperienceYears = 8,
    Rating = 4,
    ImageUrl = "/assets/doccure/img/doctor-grid/doctor-grid-08.jpg",
    SpecializationId = specializations.First(x => x.Name == "Dentist").Id,
    SpecializationName = "Dentist",
    Education = "University of Washington School of Dentistry",
    AppointmentPrice = 105m,
    Clinic = "Vyzor Dental Clinic",
    Email = "christopher.miller@vyzor.test",
    PhoneNumber = "+10000000013",
    IsActive = true
}

    };


        foreach (var doctorData in doctors)
        {
            var existingDoctor =
                await _dbContext.Doctors
                    .FirstOrDefaultAsync(
                        x => x.FullName == doctorData.FullName,
                        cancellationToken);

            if (existingDoctor == null)
            {
                _dbContext.Doctors.Add(doctorData);
            }
            else
            {
                existingDoctor.About = doctorData.About;
                existingDoctor.ExperienceYears = doctorData.ExperienceYears;
                existingDoctor.Rating = doctorData.Rating;
                existingDoctor.ImageUrl = doctorData.ImageUrl;
                existingDoctor.SpecializationId = doctorData.SpecializationId;
                existingDoctor.SpecializationName = doctorData.SpecializationName;
                existingDoctor.Education = doctorData.Education;
                existingDoctor.AppointmentPrice = doctorData.AppointmentPrice;
                existingDoctor.Clinic = doctorData.Clinic;
                existingDoctor.Email = doctorData.Email;
                existingDoctor.PhoneNumber = doctorData.PhoneNumber;
                existingDoctor.IsActive = doctorData.IsActive;
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        var result =
            await _dbContext.Doctors
                .ToListAsync(cancellationToken);

        _logger.LogInformation(
            "Doctors seeded successfully. Total doctors: {Count}",
            result.Count);

        return result;
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
        var basePlan =
            await _dbContext.SubscriptionPlans
                .FirstOrDefaultAsync(
                    x => x.Slug == "base",
                    cancellationToken);

        if (basePlan == null)
        {
            basePlan = new SubscriptionPlan
            {
                Name = "Base",
                Slug = "base",
                Description = "Basic subscription plan",
                Price = 9.99m,
                DurationDays = 30,
                DiscountPercent = 7,
                IsActive = true
            };

            _dbContext.SubscriptionPlans.Add(basePlan);
        }
        else
        {
            basePlan.Name = "Base";
            basePlan.Description = "Basic subscription plan";
            basePlan.Price = 9.99m;
            basePlan.DurationDays = 30;
            basePlan.DiscountPercent = 7;
            basePlan.IsActive = true;
        }


        var premiumPlan =
            await _dbContext.SubscriptionPlans
                .FirstOrDefaultAsync(
                    x => x.Slug == "premium",
                    cancellationToken);

        if (premiumPlan == null)
        {
            premiumPlan = new SubscriptionPlan
            {
                Name = "Premium",
                Slug = "premium",
                Description = "Premium subscription plan",
                Price = 19.99m,
                DurationDays = 30,
                DiscountPercent = 10,
                IsActive = true
            };

            _dbContext.SubscriptionPlans.Add(premiumPlan);
        }
        else
        {
            premiumPlan.Name = "Premium";
            premiumPlan.Description = "Premium subscription plan";
            premiumPlan.Price = 19.99m;
            premiumPlan.DurationDays = 30;
            premiumPlan.DiscountPercent = 10;
            premiumPlan.IsActive = true;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return (basePlan, premiumPlan);
    }


    private async Task SeedUserSubscriptionsAsync(
        string userId,
        int planId,
        CancellationToken cancellationToken)
    {
        var subscriptionExists =
            await _dbContext.UserSubscriptions
                .AnyAsync(
                    x => x.UserId == userId,
                    cancellationToken);

        if (subscriptionExists)
        {
            return;
        }

        _dbContext.UserSubscriptions.Add(
            new UserSubscription
            {
                UserId = userId,
                SubscriptionPlanId = planId,
                StartsAtUtc = DateTime.UtcNow.AddDays(-3),
                EndsAtUtc = DateTime.UtcNow.AddDays(27),
                Status = SubscriptionStatus.Active
            });

        await _dbContext.SaveChangesAsync(cancellationToken);
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

        _dbContext.AuditLogs.AddRange(
             new AuditLog
             {
                 UserId = users.Patient.Id,
                 Action = AuditAction.Register,
                 EntityName = nameof(ApplicationUser),
                 EntityId = users.Patient.Id,
                 Details = "Demo Patient account created by seed."
             },
             new AuditLog
             {
                 UserId = users.Doctor.Id,
                 Action = AuditAction.AppointmentUpdated,
                 EntityName = nameof(Appointment),
                 Details = "Demo Doctor action for dashboard."
             },
             new AuditLog
             {
                 UserId = users.Admin.Id,
                 Action = AuditAction.UserRoleChanged,
                 EntityName = nameof(ApplicationUser),
                 EntityId = users.Doctor.Id,
                 Details = "Demo admin role audit entry."
             });
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