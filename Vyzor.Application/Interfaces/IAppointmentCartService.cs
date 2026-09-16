
using Vyzor.Application.DTO.AppointmentCart;

namespace Vyzor.Application.Interfaces;

public interface IAppointmentCartService
{
    Task<IReadOnlyList<AppointmentCartItemDTO>> GetItemsAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<AppointmentCartItemDTO?> AddAsync(
        string userId,
        int doctorId,
        DateTime appointmentDate,
        CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(
        string userId,
        int cartItemId,
        CancellationToken cancellationToken = default);

    Task<int> GetCountAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<AppointmentCartSummaryDTO> GetSummaryAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<AppointmentCartPaymentResult> PayAsync(
        string userId,
        CancellationToken cancellationToken = default);
}

