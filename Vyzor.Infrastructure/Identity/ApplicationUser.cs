using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.AspNetCore.Identity;

namespace Vyzor.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}