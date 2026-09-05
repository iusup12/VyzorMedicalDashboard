using System;
using System.Collections.Generic;
using System.Text;

using Vyzor.Application.DTO.Subscription;
using Vyzor.Domain.Entities;


namespace Vyzor.Application.Mappings;

public static class SubscriptionPlanMappings
{
    public static SubscriptionPlanDTO ToDto(
        this SubscriptionPlan plan)
    {
        return new SubscriptionPlanDTO
        {
            Id = plan.Id,
            Name = plan.Name,
            DiscountPercent = plan.DiscountPercent,
            MonthlyPrice = plan.MonthlyPrice
        };
    }

    public static SubscriptionPlanEditDTO ToEditDto(
        this SubscriptionPlan plan)
    {
        return new SubscriptionPlanEditDTO
        {
            Id = plan.Id,
            Name = plan.Name,
            DiscountPercent = plan.DiscountPercent,
            MonthlyPrice = plan.MonthlyPrice
        };
    }

    public static void UpdateFromDto(
        this SubscriptionPlan plan,
        SubscriptionPlanEditDTO dto)
    {
        plan.Name = dto.Name;
        plan.DiscountPercent = dto.DiscountPercent;
        plan.MonthlyPrice = dto.MonthlyPrice;
    }
}