
using Vyzor.Application.Common;
using Vyzor.Application.Common;
using Vyzor.Application.DTO.Appointment;
using Vyzor.Application.DTO.Filters;

namespace Vyzor.Application.Interfaces;

public interface IAppointmentService
{
    Task<PagedResult<AppointmentListItemDTO>> GetPagedAsync(
        AppointmentFilterDTO filter,
        CancellationToken cancellationToken = default);

    Task<AppointmentDetailsDTO?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task CreateAsync(
     AppointmentEditDTO dto,
     string? userId,
     CancellationToken cancellationToken = default);

    Task UpdateAsync(
        AppointmentEditDTO dto,
        string? userId,
        CancellationToken cancellationToken = default);

    Task ChangeStatusAsync(
        AppointmentStatusDTO dto,
        string? userId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<AppointmentListItemDTO>> GetUserAppointmentsAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<AppointmentListItemDTO>> GetDoctorAppointmentsAsync(
        int doctorId,
        CancellationToken cancellationToken = default);
}

