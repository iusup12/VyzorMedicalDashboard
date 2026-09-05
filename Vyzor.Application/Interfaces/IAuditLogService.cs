

using Vyzor.Application.Common;
using Vyzor.Application.DTO.Audit;
using Vyzor.Application.DTO.Filters;
using Vyzor.Domain.Enums;

namespace Vyzor.Application.Interfaces;

public interface IAuditLogService
{
    Task<PagedResult<AuditLogDTO>> GetLogsAsync(
        AuditLogFilterDTO filter,
        CancellationToken cancellationToken = default);

    Task<AuditLogDetailsDTO?> GetByIdAsync(
        int id);

    Task LogAsync(
        AuditAction action,
        string entityName,
        string? entityId = null,
        string? userId = null,
        string? details = null,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default);
}

