using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using Vyzor.Application.Common;
using Vyzor.Application.Common;
using Vyzor.Application.DTO.Audit;
using Vyzor.Application.DTO.Filters;
using Vyzor.Application.DTOs.Audit;
using Vyzor.Application.Interfaces;
using Vyzor.Application.Mapping;
using Vyzor.Domain.Entities;
using Vyzor.Domain.Enums;
using Vyzor.Infrastructure.Data;


namespace Vyzor.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly AppDbContext _dbContext;

    public AuditService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task LogAsync(
        AuditAction action,
        string entityName,
        string? entityId = null,
        string? userId = null,
        string? details = null,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default)
    {
        _dbContext.AuditLogs.Add(new AuditLog
        {
            UserId = userId,
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            Details = details,
            IpAddress = ipAddress,
            UserAgent = userAgent
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<PagedResult<AuditLogDTO>> GetLogsAsync(
        AuditLogFilterDTO filter,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.AuditLogs
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.UserId))
        {
            query = query.Where(log => log.UserId == filter.UserId);
        }

        if (filter.Action.HasValue)
        {
            query = query.Where(log => log.Action == filter.Action.Value);
        }

        if (filter.FromUtc.HasValue)
        {
            query = query.Where(log => log.CreatedAtUtc >= filter.FromUtc.Value);
        }

        if (filter.ToUtc.HasValue)
        {
            query = query.Where(log => log.CreatedAtUtc <= filter.ToUtc.Value);
        }

        query = query.OrderByDescending(log => log.CreatedAtUtc);

        var totalCount = await query.CountAsync(cancellationToken);
        var pageNumber = Paging.PageNumber(filter);
        var pageSize = Paging.PageSize(filter);

        var logs = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return Paging.Result(
            logs.Select(log => log.ToDto()).ToArray(),
            filter,
            totalCount);
    }
}
