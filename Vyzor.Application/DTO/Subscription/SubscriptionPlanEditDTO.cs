using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Vyzor.Application.DTO.Subscription;

public class SubscriptionPlanEditDTO
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [Range(0, 100)]
    public decimal DiscountPercent { get; set; }

    [Range(0, 1000)]
    public decimal MonthlyPrice { get; set; }
}