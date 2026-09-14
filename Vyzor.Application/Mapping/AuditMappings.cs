using System;
using System.Collections.Generic;
using System.Text;
using Vyzor.Application.DTO.Audit;
using Vyzor.Domain.Entities;

namespace Vyzor.Application.Mapping;

public static class AuditMappings
{
    public static AuditLogDTO ToDto(this AuditLog auditLog)
    {
        return new AuditLogDTO
        {
            Id = auditLog.Id,
            UserId = auditLog.UserId,
            Action = auditLog.Action,
            EntityName = auditLog.EntityName,
            EntityId = auditLog.EntityId,
            Details = auditLog.Details,
            IpAddress = auditLog.IpAddress,
            UserAgent = auditLog.UserAgent,
            CreatedAtUtc = auditLog.CreatedAtUtc
        };
    }
}
