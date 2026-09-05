using System;
using System.Collections.Generic;
using System.Text;

namespace Vyzor.Application.DTO.Dashboard;

public class DashboardChartDTO
{
    public string Label { get; set; } = string.Empty;

    public decimal Value { get; set; }
}