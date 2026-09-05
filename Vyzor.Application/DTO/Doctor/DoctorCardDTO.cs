using System;
using System.Collections.Generic;
using System.Text;

namespace Vyzor.Application.DTO.Doctor;

public class DoctorCardDTO
{
    public int Id { get; set; }


    public string FullName { get; set; } = string.Empty;

    public int SpecializationId { get; set; }

    public string? PhotoUrl { get; set; }

    public double Rating { get; set; }

    public decimal AppointmentPrice { get; set; }
    public string SpecializationName { get; set; } = string.Empty;

}