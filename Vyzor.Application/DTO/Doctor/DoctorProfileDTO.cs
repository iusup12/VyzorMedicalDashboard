using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace Vyzor.Application.DTO.Doctor;

public class DoctorProfileDTO
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
public string FullName { get; set; } = string.Empty;

    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Phone]
    public string? PhoneNumber { get; set; }

    [StringLength(2000)]
    public string? About { get; set; }

    [StringLength(1000)]
    public string? Education { get; set; }

    [Range(0, 60)]
    public int ExperienceYears { get; set; }

    [Range(0, 10000)]
    public decimal AppointmentPrice { get; set; }

    public int SpecializationId { get; set; } 

    public string? ImageUrl { get; set; }
    public string SpecializationName { get; set; } = string.Empty;

    public string? Clinic { get; set; }

    public bool IsActive { get; set; }
}