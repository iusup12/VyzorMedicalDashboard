using System;
using System.Collections.Generic;
using System.Text;

using Vyzor.Domain.Enums;

namespace Vyzor.Application.DTO.Appointment;

public class AppointmentStatusDTO
{
    public int AppointmentId { get; set; }

    public AppointmentStatus Status { get; set; }
}