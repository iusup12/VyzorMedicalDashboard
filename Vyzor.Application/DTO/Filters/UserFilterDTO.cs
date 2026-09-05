using System;
using System.Collections.Generic;
using System.Text;

namespace Vyzor.Application.DTO.Filters;

public class UserFilterDTO
{
    public string? Search { get; set; }

    public int Page { get; set; } = 1;
}