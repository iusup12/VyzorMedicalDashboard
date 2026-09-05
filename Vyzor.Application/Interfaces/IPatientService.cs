
using Vyzor.Application.Common;
using Vyzor.Application.DTO.Filters;
using Vyzor.Application.DTO.Patient;

namespace Vyzor.Application.Interfaces;

public interface IPatientService
{
    Task<PagedResult<PatientListItemDTO>> GetListAsync(
        PatientFilterDTO filter,
        CancellationToken cancellationToken = default);


    Task<PatientDetailsDTO?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);


    Task<PatientEditDTO> CreateAsync(
        PatientEditDTO dto,
        CancellationToken cancellationToken = default);


    Task<PatientEditDTO?> UpdateAsync(
        PatientEditDTO dto,
        CancellationToken cancellationToken = default);


    Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);
}