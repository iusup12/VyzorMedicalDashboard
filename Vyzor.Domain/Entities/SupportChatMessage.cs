using System;
using Vyzor.Domain.Common;
namespace Vyzor.Domain.Entities;

public class SupportChatMessage
{
    public int Id { get; set; }

    public int SupportChatId { get; set; }

    public string SenderUserId { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public bool IsFromSupport { get; set; }

    public SupportChat SupportChat { get; set; } = null!;
}