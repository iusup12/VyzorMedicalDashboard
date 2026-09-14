using System;
using System.ComponentModel.DataAnnotations;
using Vyzor.Domain.Enums;

namespace Vyzor.Application.DTO.Appointment;

public class AppointmentEditDTO
{
    public int Id { get; set; }

    [Required]
    public int DoctorId { get; set; }

    public int PatientId { get; set; }

    [Required]
    public DateTime AppointmentDate { get; set; }

    public AppointmentStatus Status { get; set; }

    [StringLength(1000)]
    public string? About { get; set; }
}