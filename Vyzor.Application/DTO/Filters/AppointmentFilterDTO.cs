using System;
using System.Collections.Generic;
using System.Text;

using Vyzor.Domain.Enums;

namespace Vyzor.Application.DTO.Filters;

public class AppointmentFilterDTO
{
    public int? DoctorId { get; set; }

    public int? PatientId { get; set; }

    public AppointmentStatus? Status { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public int Page { get; set; } = 1;
}