using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Vyzor.Domain.Enums;

namespace Vyzor.Application.DTO.Appointment
{
    internal class UpdateAppointmentStatusDto
    {
        [EnumDataType(typeof(AppointmentStatus))]
        public AppointmentStatus Status { get; set; }
    }
}
