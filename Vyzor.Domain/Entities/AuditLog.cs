using System;
using System.Collections.Generic;
using System.Text;

using Vyzor.Domain.Common;
using Vyzor.Domain.Enums;

namespace Vyzor.Domain.Entities;

public class AuditLog : Entity
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;

    public AuditAction Action { get; set; }

    public string EntityName { get; set; } = string.Empty;
    
    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}