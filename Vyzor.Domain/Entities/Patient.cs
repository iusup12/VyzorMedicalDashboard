using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace Vyzor.Domain.Entities;

public class Patient
{
    public int Id { get; set; }

    public string? UserId { get; set; }

    public string FullName { get; set; } = string.Empty;

    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Phone]
    public string? Phone { get; set; }

    public DateTime DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? Address { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }

    // =========================
    // Navigation properties
    // =========================

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    public ICollection<UserSubscription> Subscriptions { get; set; } = new List<UserSubscription>();
}