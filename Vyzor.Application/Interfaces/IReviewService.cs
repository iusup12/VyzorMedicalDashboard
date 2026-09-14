using System;
using System.Collections.Generic;
using System.Text;
using Vyzor.Application.DTO.Review;

public interface IReviewService
{
    Task<IEnumerable<ReviewListItemDTO>> GetByDoctorAsync(int doctorId);

    Task<ReviewEditDTO?> GetByIdAsync(int id);

    Task CreateAsync(ReviewEditDTO dto);

    Task UpdateAsync(ReviewEditDTO dto);

    Task DeleteAsync(int id);
}