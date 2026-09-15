using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Vyzor.Application.Interfaces;

namespace Vyzor.Web.Hubs;

[Authorize]
public class SupportChatHub : Hub
{
    private readonly ISupportChatService _chatService;

    public SupportChatHub(ISupportChatService chatService)
    {
        _chatService = chatService;
    }

    public async Task JoinChat()
    {
        var userId = Context.UserIdentifier;

        if (string.IsNullOrEmpty(userId))
            throw new HubException("User is not authenticated.");

        var chatId = await _chatService.GetOrCreateChatIdAsync(userId);

        if (!chatId.HasValue)
            throw new HubException("Patient profile not found.");

        var groupName = GetGroupName(chatId.Value);

        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            groupName);

        await Clients.Caller.SendAsync(
            "ChatJoined",
            chatId.Value);
    }

    public async Task SendMessage(string message)
    {
        var userId = Context.UserIdentifier;

        if (string.IsNullOrEmpty(userId))
            throw new HubException("User is not authenticated.");

        if (string.IsNullOrWhiteSpace(message))
            return;

        var result = await _chatService.SendPatientMessageAsync(
            userId,
            message);

        if (result == null)
            return;

        var groupName = GetGroupName(
            result.Message.SupportChatId);

        // Сообщение пациента
        await Clients.Group(groupName).SendAsync(
            "ReceiveMessage",
            result.Message);

        // Автоматический ответ поддержки
        if (result.SupportMessage != null)
        {
            await Clients.Group(groupName).SendAsync(
                "ReceiveMessage",
                result.SupportMessage);
        }

        // Рекомендации врачей
        if (result.Doctors != null &&
            result.Doctors.Count > 0)
        {
            await Clients.Group(groupName).SendAsync(
                "DoctorRecommendation",
                new
                {
                    specialization = result.MatchedSpecialization,
                    doctors = result.Doctors
                });
        }
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;

        if (!string.IsNullOrEmpty(userId))
        {
            var chatId = await _chatService.GetChatIdAsync(userId);

            if (chatId.HasValue)
            {
                await Groups.AddToGroupAsync(
                    Context.ConnectionId,
                    GetGroupName(chatId.Value));
            }
        }

        await base.OnConnectedAsync();
    }

    private static string GetGroupName(int chatId)
    {
        return $"support-chat-{chatId}";
    }
}