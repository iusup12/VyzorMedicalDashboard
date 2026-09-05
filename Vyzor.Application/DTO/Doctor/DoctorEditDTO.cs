using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace Vyzor.Application.DTO.Doctor;

public class DoctorEditDTO
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? About { get; set; }

    [Range(0, 60)]
    public int ExperienceYears { get; set; }

    [Range(0, 100000)]
    public decimal AppointmentPrice { get; set; }

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    [Required]
    public int SpecializationId { get; set; }
    public string SpecializationName { get; set; } =string.Empty;
}