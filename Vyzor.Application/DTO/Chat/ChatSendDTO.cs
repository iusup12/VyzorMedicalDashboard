using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace Vyzor.Application.DTO.Chat;

public class ChatSendDTO
{
    [Required]
    public string ReceiverId { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Message { get; set; } = string.Empty;
}