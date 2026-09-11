using System;
using System.Collections.Generic;
using System.Text;
using Vyzor.Domain.Enums;

namespace Vyzor.Application.DTO.Audit;

public class AuditLogDTO
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public AuditAction Action { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string? Details { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}