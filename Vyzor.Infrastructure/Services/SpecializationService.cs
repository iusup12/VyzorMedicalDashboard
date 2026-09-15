
using System.Security.Claims;
using System.Text.Json;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using Vyzor.Application.DTO.Specialization;
using Vyzor.Application.Interfaces;

using Vyzor.Domain.Entities;
using Vyzor.Domain.Enums;

using Vyzor.Infrastructure.Data;

namespace Vyzor.Infrastructure.Services;

public class SpecializationService : ISpecializationService
{
    private readonly AppDbContext _context;
    private readonly IAuditService _auditService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SpecializationService(
        AppDbContext context,
        IAuditService auditService,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _auditService = auditService;
        _httpContextAccessor = httpContextAccessor;
    }

    // =========================================================
    // GET ALL
    // =========================================================

    public async Task<IEnumerable<SpecializationListItemDTO>> GetAllAsync()
    {
        return await _context.Specializations
            .AsNoTracking()
            .Select(x => new SpecializationListItemDTO
            {
                Id = x.Id,
                Name = x.Name,
                IsActive = x.IsActive
            })
            .ToListAsync();
    }


    // =========================================================
    // GET BY ID
    // =========================================================

    public async Task<SpecializationEditDTO?> GetByIdAsync(int id)
    {
        return await _context.Specializations
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new SpecializationEditDTO
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                IsActive = x.IsActive
            })
            .FirstOrDefaultAsync();
    }


    // =========================================================
    // CREATE
    // =========================================================

    public async Task CreateAsync(SpecializationEditDTO dto)
    {
        var specialization = new Specialization
        {
            Name = dto.Name,
            Description = dto.Description,
            IsActive = dto.IsActive
        };

        _context.Specializations.Add(specialization);

        await _context.SaveChangesAsync();

        var details = JsonSerializer.Serialize(new
        {
            NewValues = new
            {
                specialization.Id,
                specialization.Name,
                specialization.Description,
                specialization.IsActive
            }
        });

        await _auditService.LogAsync(
            AuditAction.SpecializationCreated,
            "Specialization",
            specialization.Id.ToString(),
            GetCurrentUserId(),
            details,
            GetIpAddress(),
            GetUserAgent());
    }


    // =========================================================
    // UPDATE
    // =========================================================

    public async Task UpdateAsync(SpecializationEditDTO dto)
    {
        var specialization = await _context.Specializations
            .FirstOrDefaultAsync(x => x.Id == dto.Id);

        if (specialization == null)
        {
            return;
        }

        // Сохраняем старые значения
        var oldValues = new
        {
            specialization.Id,
            specialization.Name,
            specialization.Description,
            specialization.IsActive
        };


        // Обновляем
        specialization.Name = dto.Name;
        specialization.Description = dto.Description;
        specialization.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();


        // Новые значения
        var newValues = new
        {
            specialization.Id,
            specialization.Name,
            specialization.Description,
            specialization.IsActive
        };


        var details = JsonSerializer.Serialize(new
        {
            OldValues = oldValues,
            NewValues = newValues
        });


        await _auditService.LogAsync(
            AuditAction.SpecializationUpdated,
            "Specialization",
            specialization.Id.ToString(),
            GetCurrentUserId(),
            details,
            GetIpAddress(),
            GetUserAgent());
    }


    // =========================================================
    // DELETE
    // =========================================================

    public async Task DeleteAsync(int id)
    {
        var specialization = await _context.Specializations
            .FirstOrDefaultAsync(x => x.Id == id);

        if (specialization == null)
        {
            return;
        }


        // Сохраняем данные перед удалением
        var oldValues = new
        {
            specialization.Id,
            specialization.Name,
            specialization.Description,
            specialization.IsActive
        };


        _context.Specializations.Remove(specialization);

        await _context.SaveChangesAsync();


        var details = JsonSerializer.Serialize(new
        {
            OldValues = oldValues
        });


        await _auditService.LogAsync(
            AuditAction.SpecializationDeleted,
            "Specialization",
            specialization.Id.ToString(),
            GetCurrentUserId(),
            details,
            GetIpAddress(),
            GetUserAgent());
    }


    // =========================================================
    // CURRENT USER
    // =========================================================

    private string? GetCurrentUserId()
    {
        return _httpContextAccessor.HttpContext?
            .User?
            .FindFirstValue(ClaimTypes.NameIdentifier);
    }


    // =========================================================
    // IP ADDRESS
    // =========================================================

    private string? GetIpAddress()
    {
        return _httpContextAccessor.HttpContext?
            .Connection?
            .RemoteIpAddress?
            .ToString();
    }


    // =========================================================
    // USER AGENT
    // =========================================================

    private string? GetUserAgent()
    {
        return _httpContextAccessor.HttpContext?
            .Request?
            .Headers["User-Agent"]
            .ToString();
    }
}

