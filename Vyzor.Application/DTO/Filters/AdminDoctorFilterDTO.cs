using System;
using System.Collections.Generic;
using System.Text;

namespace Vyzor.Application.DTO.Filters;

public class AdminDoctorFilterDTO
{
    public string? Search { get; set; }

    public int? SpecializationId { get; set; }

    public bool? IsActive { get; set; }

    public int Page { get; set; } = 1;
}