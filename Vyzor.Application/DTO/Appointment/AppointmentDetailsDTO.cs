using System;
using System.Collections.Generic;
using System.Text;

using Vyzor.Domain.Enums;

namespace Vyzor.Application.DTO.Appointment;

public class AppointmentDetailsDTO
{
    public int AppointmentId { get; set; }


    public int DoctorId { get; set; }
    public int PatientId { get; set; }


    public DateTime AppointmentDate { get; set; }

    public AppointmentStatus Status { get; set; }

    public decimal Price { get; set; }

    public decimal DiscountPercent { get; set; }

    public decimal FinalPrice { get; set; }
}