using System;
using System.Collections.Generic;
using System.Text;

namespace Vyzor.Application.DTO.Audit;

public class AuditLogDetailsDTO
{
    public string Id { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;

    public string EntityName { get; set; } = string.Empty;

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public DateTime CreatedAt { get; set; }
}