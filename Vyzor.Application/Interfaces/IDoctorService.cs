
using Vyzor.Application.Common;
using Vyzor.Application.DTO.Doctor;
using Vyzor.Application.DTO.Filters;

namespace Vyzor.Application.Interfaces;

public interface IDoctorService
{
    Task<PagedResult<DoctorListItemDTO>> GetPagedAsync(
        AdminDoctorFilterDTO filter,
        CancellationToken cancellationToken = default);


    Task<IEnumerable<DoctorCardDTO>> GetCatalogAsync(
        CancellationToken cancellationToken = default);
    Task<PagedResult<DoctorCardDTO>> GetCatalogPagedAsync(
    DoctorCatalogFilterDTO filter,
    CancellationToken cancellationToken = default);

    Task<DoctorDetailsDTO?> GetDetailsAsync(
        int id,
        CancellationToken cancellationToken = default);


    Task<DoctorProfileDTO?> GetProfileAsync(
        int id,
        CancellationToken cancellationToken = default);


    Task<DoctorEditDTO?> GetForEditAsync(
        int id,
        CancellationToken cancellationToken = default);


    Task CreateAsync(
        DoctorEditDTO dto,
        CancellationToken cancellationToken = default);


    Task UpdateAsync(
        DoctorEditDTO dto,
        CancellationToken cancellationToken = default);


    Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);
}