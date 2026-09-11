using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Vyzor.Application.DTO.Subscription
{
    public class RenewSubscriptionDTO
    {
        [Range(1, int.MaxValue)]
        public int SubscriptionPlanId { get; set; }
    }
}
