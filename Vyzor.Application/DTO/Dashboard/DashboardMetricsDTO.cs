using System;
using System.Collections.Generic;
using System.Text;
namespace Vyzor.Application.DTO.Dashboard;

public class DashboardMetricsDTO
{
    public int TotalDoctors { get; set; }

    public int TotalPatients { get; set; }

    public int TotalAppointments { get; set; }

    public int ActiveSubscriptions { get; set; }

    public decimal Revenue { get; set; }
}
