using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Vyzor.Application.DTO.Review;
using Vyzor.Application.Interfaces;
using Vyzor.Domain.Entities;
using Vyzor.Infrastructure.Data;

namespace Vyzor.Infrastructure.Services;

public class ReviewService : IReviewService
{
    private readonly AppDbContext _context;

    public ReviewService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ReviewListItemDTO>> GetByDoctorAsync(int doctorId)
    {
        return await _context.Reviews
            .Include(x => x.Patient)
            .Where(x => x.DoctorId == doctorId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new ReviewListItemDTO
            {
                Id = x.Id,
                PatientName = x.Patient != null
                    ? x.Patient.FullName
                    : string.Empty,
                Rating = x.Rating,
                Comment = x.Comment,
                CreatedAtUtc = x.CreatedAtUtc
            })
            .ToListAsync();
    }

    public async Task<ReviewEditDTO?> GetByIdAsync(int id)
    {
        return await _context.Reviews
            .Where(x => x.Id == id)
            .Select(x => new ReviewEditDTO
            {
                Id = x.Id,
                Rating = x.Rating,
                Comment = x.Comment
            })
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(ReviewEditDTO dto)
    {
        var review = new Review
        {
            Rating = dto.Rating,
            Comment = dto.Comment ?? string.Empty
        };

        _context.Reviews.Add(review);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ReviewEditDTO dto)
    {
        var review = await _context.Reviews
            .FirstOrDefaultAsync(x => x.Id == dto.Id);

        if (review == null)
            return;

        review.Rating = dto.Rating;
        review.Comment = dto.Comment ?? string.Empty;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var review = await _context.Reviews
            .FirstOrDefaultAsync(x => x.Id == id);

        if (review == null)
            return;

        _context.Reviews.Remove(review);

        await _context.SaveChangesAsync();
    }
}