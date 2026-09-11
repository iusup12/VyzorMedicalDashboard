using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Vyzor.Application.Common;
using Vyzor.Domain.Enums;

namespace Vyzor.Application.DTO.Filters;

public class AuditLogFilterDTO : PagedRequest
{
    [StringLength(450)]
    public string? UserId { get; set; }

    public AuditAction? Action { get; set; }
    public DateTime? FromUtc { get; set; }
    public DateTime? ToUtc { get; set; }
}
