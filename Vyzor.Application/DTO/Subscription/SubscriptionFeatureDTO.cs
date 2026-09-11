using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Vyzor.Application.DTO.Subscription;

public class SubscriptionFeatureDTO
{
    public int Id { get; set; }

    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string Code { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Description { get; set; }
}