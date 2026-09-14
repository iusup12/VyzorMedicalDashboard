using System;
using System.Collections.Generic;
using System.Text;
using Vyzor.Application.DTO.Specialization;

namespace Vyzor.Application.Interfaces;

public interface ISpecializationService
{
    Task<IEnumerable<SpecializationListItemDTO>> GetAllAsync();

    Task<SpecializationEditDTO?> GetByIdAsync(int id);

    Task CreateAsync(SpecializationEditDTO dto);

    Task UpdateAsync(SpecializationEditDTO dto);

    Task DeleteAsync(int id);
}