using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace Vyzor.Application.DTO.Review;

public class ReviewEditDTO
{
    public int Id { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }
    public int DoctorId { get; set; }
    public int PatientId { get; set; }
    [StringLength(1000)]
    public string? Comment { get; set; }
}