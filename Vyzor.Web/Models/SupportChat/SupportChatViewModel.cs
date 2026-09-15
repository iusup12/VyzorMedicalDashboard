using Vyzor.Application.DTO;


namespace Vyzor.Web.Models.SupportChat;

public class SupportChatViewModel
{
    public int? ChatId { get; set; }

    public List<ChatMessageDTO> Messages { get; set; } = new();
}