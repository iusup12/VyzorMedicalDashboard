using System;
using System.Collections.Generic;
using System.Text;

using Vyzor.Application.DTO.Subscription;

namespace Vyzor.Application.Interfaces;

public interface ISubscriptionPlanService
{
    Task<IEnumerable<SubscriptionPlanDTO>> GetPlansAsync();

    Task<SubscriptionPlanEditDTO?> GetPlanAsync(int id);

    Task CreatePlanAsync(SubscriptionPlanEditDTO dto);

    Task UpdatePlanAsync(SubscriptionPlanEditDTO dto);

    Task DeletePlanAsync(int id);

    Task<UserSubscriptionDTO?> GetUserSubscriptionAsync(string userId);

    Task SubscribeAsync(string userId, int planId);

    Task CancelSubscriptionAsync(string userId);
}