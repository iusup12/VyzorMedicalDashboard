using System;
using System.Collections.Generic;
using System.Text;
using Vyzor.Application.DTO.Chat;

public interface IChatService
{
    Task<IEnumerable<ChatMessageDTO>> GetConversationAsync(
        string senderId,
        string receiverId);

    Task<ChatMessageDTO> SendMessageAsync(
        ChatSendDTO dto,
        string senderId);

    Task<int> GetUnreadCountAsync(string userId);

    Task MarkAsReadAsync(
        string userId,
        string senderId);
}
