using Vyzor.Application.DTO;
using Vyzor.Application.DTO.Chat;


namespace Vyzor.Application.Interfaces;

public interface ISupportChatService
{
    Task<int?> GetChatIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<int?> GetOrCreateChatIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ChatMessageDTO>> GetMessagesAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<SupportChatResultDTO?> SendPatientMessageAsync(
        string userId,
        string message,
        CancellationToken cancellationToken = default);
}