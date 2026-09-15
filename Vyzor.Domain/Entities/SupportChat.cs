using System;
using System.Collections.Generic;
using Vyzor.Domain.Common;
namespace Vyzor.Domain.Entities;

public class SupportChat
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? LastMessageAtUtc { get; set; }

    public bool IsClosed { get; set; }

    public Patient Patient { get; set; } = null!;

    public ICollection<SupportChatMessage> Messages { get; set; }
        = new List<SupportChatMessage>();
}