using System;
using System.Collections.Generic;
using System.Text;
using Vyzor.Application.DTO.Subscription;
using Vyzor.Domain.Entities;
using Vyzor.Domain.Enums;

namespace Vyzor.Application.Mappings;

public static class UserSubscriptionMappings
{
    public static UserSubscriptionDTO ToDto(
        this UserSubscription subscription)
    {
        return new UserSubscriptionDTO
        {
            PlanId = subscription.SubscriptionPlanId,
            PlanName = subscription.SubscriptionPlan?.Name ?? string.Empty,
            DiscountPercent =
                subscription.SubscriptionPlan?.DiscountPercent ?? 0,

            StartDate = subscription.StartDate,
            EndDate = subscription.EndDate,

            IsActive = subscription.Status == SubscriptionStatus.Active
        };
    }

    public static bool IsActive(this UserSubscription subscription)
    {
        return subscription.Status == SubscriptionStatus.Active
            && subscription.EndDate > DateTime.UtcNow;
    }
}
