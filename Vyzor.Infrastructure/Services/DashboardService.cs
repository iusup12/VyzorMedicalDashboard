using Microsoft.EntityFrameworkCore;
using Vyzor.Application.DTO.Dashboard;
using Vyzor.Application.Interfaces;
using Vyzor.Infrastructure.Data;

namespace Vyzor.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardMetricsDTO> GetMetricsAsync(
        CancellationToken cancellationToken = default)
    {
        return new DashboardMetricsDTO
        {
            TotalDoctors = await _context.Doctors
                .CountAsync(cancellationToken),

            TotalPatients = await _context.Patients
                .CountAsync(cancellationToken),

            TotalAppointments = await _context.Appointments
                .CountAsync(cancellationToken),

            ActiveSubscriptionsCount = await _context.UserSubscriptions
                .CountAsync(
                    x => x.EndsAtUtc >= DateTime.UtcNow,
                    cancellationToken),

            AuditLogsCount = await _context.AuditLogs
                .CountAsync(cancellationToken)
        };
    }
}