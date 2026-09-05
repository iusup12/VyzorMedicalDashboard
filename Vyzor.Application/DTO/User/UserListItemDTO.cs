using System;
using System.Collections.Generic;
using System.Text;

namespace Vyzor.Application.DTO.User;

public class UserListItemDTO
{
    public int Id { get; set; } 

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public string Role { get; set; } = string.Empty;

    public bool EmailConfirmed { get; set; }

    public bool IsBlocked { get; set; }

    public DateTime CreatedAt { get; set; }
}