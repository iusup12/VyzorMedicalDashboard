using System;
using Vyzor.Application.Common;
using Vyzor.Domain.Enums;

namespace Vyzor.Application.DTO.Filters;

public class AppointmentFilterDTO : PagedRequest
{
    public int? DoctorId { get; set; }

    public int? PatientId { get; set; }

    public AppointmentStatus? Status { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }
}