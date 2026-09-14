using System;
using System.Collections.Generic;
using System.Text;
using Vyzor.Application.DTO.Review;
using Vyzor.Domain.Entities;

namespace Vyzor.Application.Mappings;

public static class ReviewMappings
{
    public static ReviewListItemDTO ToListItemDto(
        this Review review,
        string patientName)
    {
        return new ReviewListItemDTO
        {
            Id = review.Id,
            PatientName = patientName,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAtUtc = review.CreatedAtUtc
        };
    }

    public static ReviewEditDTO ToEditDto(this Review review)
    {
        return new ReviewEditDTO
        {
            Id = review.Id,
            Rating = review.Rating,
            Comment = review.Comment
        };
    }

    public static void UpdateFromDto(
        this Review review,
        ReviewEditDTO dto)
    {
        review.Rating = dto.Rating;
        review.Comment = dto.Comment ?? string.Empty;
    }
}
