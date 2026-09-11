using System;
using System.Collections.Generic;
using System.Text;

using Vyzor.Application.DTO.Subscription;
using Vyzor.Domain.Entities;


namespace Vyzor.Application.Mappings;

public static class SubscriptionMappings
{
    public static SubscriptionPlanDTO ToDto(this SubscriptionPlan plan)
    {
        return new SubscriptionPlanDTO
        {
            Id = plan.Id,
            Name = plan.Name,
            Slug = plan.Slug,
            Description = plan.Description,
            Price = plan.Price,
            DurationDays = plan.DurationDays,
            DiscountPercent = plan.DiscountPercent,
            IsActive = plan.IsActive,
            Features = plan.Features.Select(feature => feature.ToDto()).ToArray()
        };
    }

    public static SubscriptionFeatureDTO ToDto(this SubscriptionFeature feature)
    {
        return new SubscriptionFeatureDTO
        {
            Id = feature.Id,
            Name = feature.Name,
            Code = feature.Code,
            Description = feature.Description
        };
    }

    public static UserSubscriptionDTO ToDto(this UserSubscription subscription)
    {
        return new UserSubscriptionDTO
        {
            Id = subscription.Id,
            UserId = subscription.UserId,
            SubscriptionPlanId = subscription.SubscriptionPlanId,
            PlanName = subscription.SubscriptionPlan?.Name ?? string.Empty,
            StartsAtUtc = subscription.StartsAtUtc,
            EndsAtUtc = subscription.EndsAtUtc,
            Status = subscription.Status
        };
    }

    public static void ApplyTo(this SubscriptionPlanDTO dto, SubscriptionPlan plan)
    {
        plan.Name = dto.Name;
        plan.Slug = dto.Slug;
        plan.Description = dto.Description;
        plan.Price = dto.Price;
        plan.DurationDays = dto.DurationDays;
        plan.DiscountPercent = dto.DiscountPercent;
        plan.IsActive = dto.IsActive;
        plan.UpdatedAtUtc = DateTime.UtcNow;
    }
}
