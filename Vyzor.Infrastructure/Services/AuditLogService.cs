using Vyzor.Application.Common;
using Microsoft.EntityFrameworkCore;
using Vyzor.Application.Common;
using Vyzor.Application.DTO.Audit;
using Vyzor.Application.DTO.Filters;
using Vyzor.Application.Interfaces;
using Vyzor.Domain.Entities;
using Vyzor.Domain.Enums;
using Vyzor.Infrastructure.Data;

namespace Vyzor.Infrastructure.Services;

public class AuditLogService : IAuditLogService
{
    private readonly AppDbContext _context;


    public AuditLogService(AppDbContext context)
    {
        _context = context;
    }



    public async Task<PagedResult<AuditLogDTO>> GetLogsAsync(
        AuditLogFilterDTO filter,
        CancellationToken cancellationToken = default)
    {
        var query = _context.AuditLogs
            .AsQueryable();



        // Фильтр по пользователю
        if (!string.IsNullOrWhiteSpace(filter.UserName))
        {
            query = query.Where(x =>
                x.UserName.Contains(filter.UserName));
        }



        // Фильтр по действию
        if (!string.IsNullOrWhiteSpace(filter.Action))
        {
            query = query.Where(x =>
                x.Action.ToString() == filter.Action);
        }



        // Дата от
        if (filter.FromDate.HasValue)
        {
            query = query.Where(x =>
                x.CreatedAt >= filter.FromDate.Value);
        }



        // Дата до
        if (filter.ToDate.HasValue)
        {
            query = query.Where(x =>
                x.CreatedAt <= filter.ToDate.Value);
        }



        int pageSize = 10;


        var totalCount =
            await query.CountAsync(cancellationToken);



        var items =
            await query
                .OrderByDescending(x => x.CreatedAt)

                .Skip((filter.Page - 1) * pageSize)

                .Take(pageSize)

                .Select(x => new AuditLogDTO
                {
                    Id = x.Id.ToString(),

                    UserName = x.UserName,

                    Action = x.Action.ToString(),

                    EntityName = x.EntityName,

                    CreatedAt = x.CreatedAt
                })

                .ToListAsync(cancellationToken);



        return new PagedResult<AuditLogDTO>
        {
            Items = items,

            TotalCount = totalCount,

            PageNumber = filter.Page,

            PageSize = pageSize
        };
    }





    public async Task<AuditLogDetailsDTO?> GetByIdAsync(
        int id)
    {
        return await _context.AuditLogs

            .Where(x => x.Id == id)

            .Select(x => new AuditLogDetailsDTO
            {
                Id = x.Id.ToString(),

                UserName = x.UserName,

                Action = x.Action.ToString(),

                EntityName = x.EntityName,

                OldValues = x.OldValues,

                NewValues = x.NewValues,

                CreatedAt = x.CreatedAt

            })

            .FirstOrDefaultAsync();
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


        var log = new AuditLog
        {
            UserId = userId ?? string.Empty,

            UserName = string.Empty,

            Action = action,

            EntityName = entityName,

          
            CreatedAt = DateTime.UtcNow
        };



        _context.AuditLogs.Add(log);


        await _context.SaveChangesAsync(cancellationToken);
    }
}