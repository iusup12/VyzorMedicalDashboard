namespace Vyzor.Application.DTO.Doctor;

public class DoctorDetailsDTO
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string? About { get; set; }

    public string? Education { get; set; }

    public int ExperienceYears { get; set; }

    public decimal AppointmentPrice { get; set; }

    public int SpecializationId { get; set; }

    public string SpecializationName { get; set; } = string.Empty;

    public string PhotoUrl { get; set; } = string.Empty;

    public double Rating { get; set; }
    public string? Clinic { get; set; }
}