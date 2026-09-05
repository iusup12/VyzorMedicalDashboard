using System;
using System.Collections.Generic;
using System.Text;

namespace Vyzor.Application.DTO.Patient;

public class PatientListItemDTO
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}