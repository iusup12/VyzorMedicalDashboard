using System;
using System.Collections.Generic;
using System.Text;

using Vyzor.Domain.Enums;

namespace Vyzor.Application.DTO.Appointment;

public class AppointmentListItemDTO
{
    public int Id { get; set; }

    public int DoctorId { get; set; } 

    public int PatientId { get; set; }

    public string PatientName { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;

    public DateTime AppointmentDate { get; set; }

    public AppointmentStatus Status { get; set; }
}