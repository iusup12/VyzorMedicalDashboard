using System;
using System.Collections.Generic;
using System.Text;

namespace Vyzor.Application.DTO.User;

public class UserDetailsDTO
{
    public int Id { get; set; } 

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}";

    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public string Role { get; set; } = string.Empty;

    public bool EmailConfirmed { get; set; }

    public bool IsBlocked { get; set; }

    public DateTime CreatedAt { get; set; }

    public int AppointmentsCount { get; set; }

    public string? ActiveSubscription { get; set; }
}