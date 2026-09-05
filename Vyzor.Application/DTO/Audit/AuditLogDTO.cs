using System;
using System.Collections.Generic;
using System.Text;

namespace Vyzor.Application.DTO.Audit;

public class AuditLogDTO
{
    public string Id { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;

    public string EntityName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}