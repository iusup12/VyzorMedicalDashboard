using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Vyzor.Application.DTO.Subscription;

public class SubscriptionPlanDTO
{

    public int Id { get; set; }

    [Required]
    [StringLength(80)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Slug { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(1, 3660)]
    public int DurationDays { get; set; }

    [Range(0, 100)]
    public decimal DiscountPercent { get; set; }

    public bool IsActive { get; set; } = true;
    public IReadOnlyList<SubscriptionFeatureDTO> Features { get; set; } = Array.Empty<SubscriptionFeatureDTO>();
}