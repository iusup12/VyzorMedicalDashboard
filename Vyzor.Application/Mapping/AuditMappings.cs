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
            Id = auditLog.UserId,
            UserName = auditLog.UserName,
            Action = auditLog.Action.ToString(),
            EntityName = auditLog.EntityName
        };
    }

    public static AuditLogDetailsDTO ToDetailsDto(this AuditLog auditLog)
    {
        return new AuditLogDetailsDTO
        {
            Id = auditLog.UserId,
            UserName = auditLog.UserName,
            Action = auditLog.Action.ToString(),
            EntityName = auditLog.EntityName,
            OldValues = null,
            NewValues = auditLog.Description,
            CreatedAt = DateTime.MinValue
        };
    }
}

