using System;
using System.Collections.Generic;
using System.Text;

namespace Vyzor.Application.DTO.Subscription;

public class SubscriptionPlanDTO
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal DiscountPercent { get; set; }

    public decimal MonthlyPrice { get; set; }
}