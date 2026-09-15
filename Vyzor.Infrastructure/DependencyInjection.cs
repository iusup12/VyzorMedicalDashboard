using Vyzor.Application.Interfaces;
using Vyzor.Infrastructure.Data;
using Vyzor.Infrastructure.Identity;
using Vyzor.Infrastructure.Options;
using Vyzor.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Vyzor.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();


        services.Configure<MongoOptions>(
            configuration.GetSection("Mongo"));

        var mongoConnectionString =
            configuration.GetConnectionString("Mongo");

        services.AddSingleton<IMongoClient>(
            new MongoClient(mongoConnectionString!));

        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IDoctorService, DoctorService>();
        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<IAppointmentService, AppointmentService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<ISpecializationService, SpecializationService>();
        services.AddScoped<ISupportChatService, SupportChatService>();
        services.AddScoped<IDashboardService, DashboardService>();

        services.AddScoped<ITechnicalLogService, TechnicalLogService>();

        services.AddScoped<DatabaseSeeder>();

        return services;
    }
}