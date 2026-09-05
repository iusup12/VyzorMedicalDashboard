using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace Vyzor.Application.DTO.Patient;

public class PatientEditDTO
{
    public int Id { get; set; }

    public string? UserId { get; set; }

    [Required]
    [StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Phone]
    public string? Phone { get; set; }

    [Required]
    public DateTime DateOfBirth { get; set; }

    [StringLength(20)]
    public string? Gender { get; set; }

    [StringLength(300)]
    public string? Address { get; set; }
}