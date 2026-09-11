using System;
using System.Collections.Generic;
using System.Text;
using Vyzor.Domain.Common;

namespace Vyzor.Domain.Entities
{
    public class SubscriptionFeature : Entity
    {
        public int SubscriptionPlanId { get; set; }
        public SubscriptionPlan? SubscriptionPlan { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
