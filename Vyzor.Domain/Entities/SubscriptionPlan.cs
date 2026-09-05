using System;
using System.Collections.Generic;
using System.Text;

using Vyzor.Domain.Common;

namespace Vyzor.Domain.Entities;

public class SubscriptionPlan : Entity
{
    public string Name { get; set; } = string.Empty;

    public decimal DiscountPercent { get; set; }

    public decimal MonthlyPrice { get; set; }
}
