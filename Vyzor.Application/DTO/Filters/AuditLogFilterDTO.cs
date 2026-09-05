using System;
using System.Collections.Generic;
using System.Text;

namespace Vyzor.Application.DTO.Filters;

public class AuditLogFilterDTO
{
    public string? UserName { get; set; }

    public string? Action { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public int Page { get; set; } = 1;
}