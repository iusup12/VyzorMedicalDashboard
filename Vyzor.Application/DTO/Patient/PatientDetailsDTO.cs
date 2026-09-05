using System;
using System.Collections.Generic;
using System.Text;

namespace Vyzor.Application.DTO.Patient;

public class PatientDetailsDTO
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Address { get; set; }
}