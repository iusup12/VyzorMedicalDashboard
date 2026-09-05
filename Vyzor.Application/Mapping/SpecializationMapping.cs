using System;
using System.Collections.Generic;
using System.Text;
using Vyzor.Application.DTO.Specialization;
using Vyzor.Domain.Entities;

namespace Vyzor.Application.Mappings;

public static class SpecializationMappings
{
    public static SpecializationListItemDTO ToListItemDto(
        this Specialization specialization)
    {
        return new SpecializationListItemDTO
        {
            Id = specialization.Id,
            Name = specialization.Name,
          
            IsActive = specialization.IsActive
        };
    }

    public static SpecializationEditDTO ToEditDto(
        this Specialization specialization)
    {
        return new SpecializationEditDTO
        {
            Id = specialization.Id,
            Name = specialization.Name,
            Description = specialization.Description,
            IsActive = specialization.IsActive
        };
    }

    public static void UpdateFromDto(
        this Specialization specialization,
        SpecializationEditDTO dto)
    {
        specialization.Name = dto.Name;
        specialization.Description = dto.Description;
        specialization.IsActive = dto.IsActive;
    }
}
