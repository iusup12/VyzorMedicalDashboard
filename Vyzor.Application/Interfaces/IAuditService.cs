

using Vyzor.Application.Common;
using Vyzor.Application.DTO;
using Vyzor.Domain.Enums;
using Vyzor.Application.DTO.Audit;
using Vyzor.Application.DTO.Filters;

namespace Vyzor.Application.Interfaces;

public interface IAuditService
{
    Task LogAsync(
        AuditAction action,
        string entityName,
        string? entityId = null,
        string? userId = null,
        string? details = null,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default);

    Task<PagedResult<AuditLogDTO>> GetLogsAsync(
        AuditLogFilterDTO filter,
        CancellationToken cancellationToken = default);
}