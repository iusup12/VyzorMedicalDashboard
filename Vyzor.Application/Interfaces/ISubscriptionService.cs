using System;
using System.Collections.Generic;
using System.Text;
using Vyzor.Application.Common;
using Vyzor.Application.DTO.Subscription;

namespace Vyzor.Application.Interfaces;

public interface ISubscriptionService
{
    Task<IReadOnlyList<SubscriptionPlanDTO>> GetActivePlansAsync(
        CancellationToken cancellationToken = default);

    Task<UserSubscriptionDTO?> GetCurrentUserSubscriptionAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<UserSubscriptionDTO> RenewAsync(
        string userId,
        RenewSubscriptionDTO dto,
        CancellationToken cancellationToken = default);

    Task<PagedResult<SubscriptionPlanDTO>> GetPlansForAdminAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default);

    Task<SubscriptionPlanDTO?> GetPlanAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<int> CreatePlanAsync(
        SubscriptionPlanDTO dto,
        string adminUserId,
        CancellationToken cancellationToken = default);

    Task UpdatePlanAsync(
        SubscriptionPlanDTO dto,
        string adminUserId,
        CancellationToken cancellationToken = default);

    Task DeletePlanAsync(
        int id,
        string adminUserId,
        CancellationToken cancellationToken = default);

    Task ActivatePlanAsync(
        int id,
        string adminUserId,
        CancellationToken cancellationToken = default);
}

