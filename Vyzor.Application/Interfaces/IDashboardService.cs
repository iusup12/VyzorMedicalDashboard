using Vyzor.Application.DTO.Dashboard;

namespace Vyzor.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardMetricsDTO> GetMetricsAsync(
        CancellationToken cancellationToken = default);
}