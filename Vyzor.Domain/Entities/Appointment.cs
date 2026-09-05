using System;
using System.Collections.Generic;
using System.Text;

using Vyzor.Domain.Common;
using Vyzor.Domain.Enums;

namespace Vyzor.Domain.Entities;

public class Appointment : Entity
{
    public int PatientId { get; set; } 
    public Patient? Patient { get; set; }
    public int AppointmentId { get; set; }

    public string PatientName { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;

    public int DoctorId { get; set; }

    public Doctor? Doctor { get; set; }

    public DateTime AppointmentDate { get; set; }

    public AppointmentStatus Status { get; set; }

    public decimal Price { get; set; }

    public decimal DiscountPercent { get; set; }

    public decimal FinalPrice { get; set; }

    public string? About {  get; set; }
}