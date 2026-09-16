using Vyzor.Application.Common;
using Vyzor.Application.DTO.Subscription;

namespace Vyzor.Web.Areas.Admin.Models;

public class AdminSubscriptionPlansIndexViewModel
{
    public PagedRequest Request { get; set; } = new();

    public PagedResult<SubscriptionPlanDTO> Plans { get; set; } = default!;
}