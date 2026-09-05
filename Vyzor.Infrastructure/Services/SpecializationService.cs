using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Vyzor.Application.DTO.Specialization;
using Vyzor.Application.Interfaces;
using Vyzor.Domain.Entities;
using Vyzor.Infrastructure.Data;

namespace Vyzor.Infrastructure.Services;

public class SpecializationService : ISpecializationService
{
    private readonly AppDbContext _context;

    public SpecializationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SpecializationListItemDTO>> GetAllAsync()
    {
        return await _context.Specializations
            .Select(x => new SpecializationListItemDTO
            {
                Id = x.Id,
                Name = x.Name,
                IsActive = x.IsActive
            })
            .ToListAsync();
    }

    public async Task<SpecializationEditDTO?> GetByIdAsync(int id)
    {
        return await _context.Specializations
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
    }

    public async Task UpdateAsync(SpecializationEditDTO dto)
    {
        var specialization = await _context.Specializations
            .FirstOrDefaultAsync(x => x.Id == dto.Id);

        if (specialization == null)
            return;

        specialization.Name = dto.Name;
        specialization.Description = dto.Description;
        specialization.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var specialization = await _context.Specializations
            .FirstOrDefaultAsync(x => x.Id == id);

        if (specialization == null)
            return;

        _context.Specializations.Remove(specialization);

        await _context.SaveChangesAsync();
    }
}