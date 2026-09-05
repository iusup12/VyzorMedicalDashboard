using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Vyzor.Application.DTO.Subscription;
using Vyzor.Application.Interfaces;
using Vyzor.Application.Mappings;
using Vyzor.Domain.Entities;
using Vyzor.Domain.Enums;
using Vyzor.Infrastructure.Data;

namespace Vyzor.Infrastructure.Services;

public class SubscriptionPlanService : ISubscriptionPlanService
{
    private readonly AppDbContext _context;

    public SubscriptionPlanService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SubscriptionPlanDTO>> GetPlansAsync()
    {
        return await _context.SubscriptionPlans
            .Select(x => x.ToDto())
            .ToListAsync();
    }

    public async Task<SubscriptionPlanEditDTO?> GetPlanAsync(int id)
    {
        var entity = await _context.SubscriptionPlans
            .FirstOrDefaultAsync(x => x.Id == id);

        return entity?.ToEditDto();
    }

    public async Task CreatePlanAsync(SubscriptionPlanEditDTO dto)
    {
        var entity = new SubscriptionPlan
        {
            Name = dto.Name,
            DiscountPercent = dto.DiscountPercent,
            MonthlyPrice = dto.MonthlyPrice
        };

        _context.SubscriptionPlans.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePlanAsync(SubscriptionPlanEditDTO dto)
    {
        var entity = await _context.SubscriptionPlans
            .FirstOrDefaultAsync(x => x.Id == dto.Id);

        if (entity == null)
            return;

        entity.UpdateFromDto(dto);

        await _context.SaveChangesAsync();
    }

    public async Task DeletePlanAsync(int id)
    {
        var entity = await _context.SubscriptionPlans
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null)
            return;

        _context.SubscriptionPlans.Remove(entity);

        await _context.SaveChangesAsync();
    }

    public async Task<UserSubscriptionDTO?> GetUserSubscriptionAsync(
        string userId)
    {
        var subscription = await _context.UserSubscriptions
            .Include(x => x.SubscriptionPlan)
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                x.Status == SubscriptionStatus.Active);

        if (subscription == null)
            return null;

        return new UserSubscriptionDTO
        {
            PlanId = subscription.SubscriptionPlanId,
            PlanName = subscription.SubscriptionPlan?.Name ?? string.Empty,
            DiscountPercent = subscription.SubscriptionPlan?.DiscountPercent ?? 0,
            StartDate = subscription.StartDate,
            EndDate = subscription.EndDate,
            IsActive = subscription.Status == SubscriptionStatus.Active
        };
    }

    public async Task SubscribeAsync(
        string userId,
        int planId)
    {
        var subscription = new UserSubscription
        {
            UserId = userId,
            SubscriptionPlanId = planId,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(1),
            Status = SubscriptionStatus.Active
        };

        _context.UserSubscriptions.Add(subscription);

        await _context.SaveChangesAsync();
    }

    public async Task CancelSubscriptionAsync(
        string userId)
    {
        var subscription = await _context.UserSubscriptions
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                x.Status == SubscriptionStatus.Active);

        if (subscription == null)
            return;

        subscription.Status = SubscriptionStatus.Cancelled;

        await _context.SaveChangesAsync();
    }
}
