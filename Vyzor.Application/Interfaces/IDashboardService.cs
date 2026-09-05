using System;
using System.Collections.Generic;
using System.Text;
using Vyzor.Application.DTO.Dashboard;

public interface IDashboardService
{
    Task<DashboardMetricsDTO> GetMetricsAsync(CancellationToken cancellationToken = default);
}

