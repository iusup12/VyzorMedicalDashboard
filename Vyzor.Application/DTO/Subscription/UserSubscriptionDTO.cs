using System;
using System.Collections.Generic;
using System.Text;

namespace Vyzor.Application.DTO.Subscription;

public class UserSubscriptionDTO
{
    public int PlanId { get; set; }

    public string PlanName { get; set; } = string.Empty;

    public decimal DiscountPercent { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; }
}