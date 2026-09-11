using System;
using System.Collections.Generic;
using System.Text;
using Vyzor.Domain.Enums;

namespace Vyzor.Application.DTO.Subscription;

public class UserSubscriptionDTO
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int SubscriptionPlanId { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public DateTime StartsAtUtc { get; set; }
    public DateTime EndsAtUtc { get; set; }
    public SubscriptionStatus Status { get; set; }
}