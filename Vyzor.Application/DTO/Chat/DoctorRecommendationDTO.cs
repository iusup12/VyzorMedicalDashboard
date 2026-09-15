namespace Vyzor.Application.DTO.Chat;

public class DoctorRecommendationDTO
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string SpecializationName { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public decimal AppointmentPrice { get; set; }

    public double Rating { get; set; }
}