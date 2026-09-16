using System.Security.Claims;
using Vyzor.Application.Interfaces;
using Vyzor.Infrastructure.Interfaces;

namespace Vyzor.Web.Middleware;

public class TechnicalLogMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TechnicalLogMiddleware> _logger;

    public TechnicalLogMiddleware(
        RequestDelegate next,
        ILogger<TechnicalLogMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ITechnicalLogService technicalLogService)
    {
        try
        {
            await _next(context);

            var statusCode = context.Response.StatusCode;

            // Пока не логируем статические файлы
            if (IsStaticFile(context.Request.Path))
                return;

            var level = statusCode switch
            {
                >= 500 => "Error",
                >= 400 => "Warning",
                _ => "Information"
            };

            await technicalLogService.LogAsync(
                level,
                $"HTTP {statusCode}",
                $"{context.Request.Method} {context.Request.Path}{context.Request.QueryString}",
                context.User.FindFirstValue(ClaimTypes.NameIdentifier),
                null,
                CancellationToken.None);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Unhandled exception in request.");

            try
            {
                await technicalLogService.LogAsync(
                    "Error",
                    "Unhandled exception",
                    $"{context.Request.Method} {context.Request.Path}",
                    context.User.FindFirstValue(
                        ClaimTypes.NameIdentifier),
                    exception.ToString(),
                    CancellationToken.None);
            }
            catch (Exception logException)
            {
                _logger.LogWarning(
                    logException,
                    "Failed to save technical log.");
            }

            // Очень важно — не пытаться здесь менять Response
            throw;
        }
    }

    private static bool IsStaticFile(PathString path)
    {
        var value = path.Value;

        if (string.IsNullOrWhiteSpace(value))
            return false;

        return value.EndsWith(".css", StringComparison.OrdinalIgnoreCase)
            || value.EndsWith(".js", StringComparison.OrdinalIgnoreCase)
            || value.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
            || value.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
            || value.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
            || value.EndsWith(".gif", StringComparison.OrdinalIgnoreCase)
            || value.EndsWith(".svg", StringComparison.OrdinalIgnoreCase)
            || value.EndsWith(".ico", StringComparison.OrdinalIgnoreCase)
            || value.EndsWith(".woff", StringComparison.OrdinalIgnoreCase)
            || value.EndsWith(".woff2", StringComparison.OrdinalIgnoreCase);
    }
}