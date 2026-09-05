using System;
using System.Collections.Generic;
using System.Text;
using Vyzor.Domain.Common;
using Vyzor.Domain.Enums;

namespace Vyzor.Domain.Entities;

public class UserSubscription : Entity
{
    public string UserId { get; set; } = string.Empty;

    public int SubscriptionPlanId { get; set; }

    public SubscriptionPlan? SubscriptionPlan { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public SubscriptionStatus Status { get; set; }
}