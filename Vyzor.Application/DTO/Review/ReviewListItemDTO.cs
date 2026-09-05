using System;
using System.Collections.Generic;
using System.Text;

namespace Vyzor.Application.DTO.Review;

public class ReviewListItemDTO
{
    public int Id { get; set; }

    public string PatientName { get; set; } = string.Empty;

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}