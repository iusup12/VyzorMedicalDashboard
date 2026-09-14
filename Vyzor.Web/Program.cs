
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Vyzor.Infrastructure;
using Vyzor.Web.Authorization;
using Vyzor.Web.Hubs;
using Vyzor.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------------
// Localization
// ------------------------------------------------------------

builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});

// ------------------------------------------------------------
// MVC
// ------------------------------------------------------------

builder.Services
    .AddControllersWithViews(options =>
    {
        
        options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
    })
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

// ------------------------------------------------------------
// SignalR
// ------------------------------------------------------------

builder.Services.AddSignalR();

// ------------------------------------------------------------
// Response Compression
// ------------------------------------------------------------

builder.Services.AddResponseCompression();

// ------------------------------------------------------------
// Infrastructure
// ------------------------------------------------------------

builder.Services.AddInfrastructure(builder.Configuration);

// ------------------------------------------------------------
// Authentication / Identity
// ------------------------------------------------------------

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";

    // Для AJAX-запросов не делаем обычный redirect
    // на страницу Login.
    options.Events.OnRedirectToLogin = context =>
    {
        if (IsAjaxRequest(context.Request))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

            return context.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = "Login is required.",
                loginUrl = context.RedirectUri
            });
        }

        context.Response.Redirect(context.RedirectUri);

        return Task.CompletedTask;
    };

    options.Events.OnRedirectToAccessDenied = context =>
    {
        if (IsAjaxRequest(context.Request))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;

            return context.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = "Access denied."
            });
        }

        context.Response.Redirect(context.RedirectUri);

        return Task.CompletedTask;
    };
});

// ------------------------------------------------------------
// Authorization
// ------------------------------------------------------------

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        AppPolicies.AdminOnly,
        policy => policy.RequireRole(AppRoles.Admin));

    options.AddPolicy(
        AppPolicies.DoctorOnly,
        policy => policy.RequireRole(AppRoles.Doctor));

    options.AddPolicy(
        AppPolicies.PatientOnly,
        policy => policy.RequireRole(AppRoles.Patient));

    options.AddPolicy(
        AppPolicies.DoctorOrAdmin,
        policy => policy.RequireRole(
            AppRoles.Doctor,
            AppRoles.Admin));

    options.AddPolicy(
        AppPolicies.PatientOrAdmin,
        policy => policy.RequireRole(
            AppRoles.Patient,
            AppRoles.Admin));
});

// ------------------------------------------------------------
// Build
// ------------------------------------------------------------

var app = builder.Build();

// ------------------------------------------------------------
// Localization configuration
// ------------------------------------------------------------

var supportedCultures = new[]
{
    new CultureInfo("en"),
    new CultureInfo("ru")
};

var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en"),

    SupportedCultures = supportedCultures,

    SupportedUICultures = supportedCultures
};

localizationOptions.RequestCultureProviders =
[
    new CookieRequestCultureProvider(),
    new QueryStringRequestCultureProvider(),
    new AcceptLanguageHeaderRequestCultureProvider()
];

// ------------------------------------------------------------
// Error handling
// ------------------------------------------------------------

app.UseExceptionHandler("/Errors/500");

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseResponseCompression();
}

// ------------------------------------------------------------
// HTTP pipeline
// ------------------------------------------------------------

app.UseHttpsRedirection();

app.UseStatusCodePagesWithReExecute(
    "/Errors/StatusCode",
    "?code={0}");

app.UseRequestLocalization(localizationOptions);

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();


app.UseMiddleware<TechnicalLogMiddleware>();

app.UseAuthorization();

// ------------------------------------------------------------
// Areas
// ------------------------------------------------------------

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

// ------------------------------------------------------------
// Default route
// ------------------------------------------------------------

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Doctors}/{action=Index}/{id?}");


// ------------------------------------------------------------
// Appointment SignalR Hub
// ------------------------------------------------------------

app.MapHub<AppointmentHub>("/hubs/appointments");

// ------------------------------------------------------------
// Run
// ------------------------------------------------------------

app.Run();

// ------------------------------------------------------------
// AJAX helper
// ------------------------------------------------------------

static bool IsAjaxRequest(HttpRequest request)
{
    return string.Equals(
        request.Headers["X-Requested-With"].ToString(),
        "XMLHttpRequest",
        StringComparison.OrdinalIgnoreCase);
}
