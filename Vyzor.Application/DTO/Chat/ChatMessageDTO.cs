using System;
using System.Collections.Generic;
using System.Text;

namespace Vyzor.Application.DTO.Chat;

public class ChatMessageDTO
{
    public string SenderId { get; set; } = string.Empty;

    public string SenderName { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public DateTime SentAt { get; set; }
}