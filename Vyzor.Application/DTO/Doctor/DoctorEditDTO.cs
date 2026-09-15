
using System.ComponentModel.DataAnnotations;

namespace Vyzor.Application.DTO.Doctor;

public class DoctorEditDTO
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(200)]
    public string FullName { get; set; } = string.Empty;

    public string? About { get; set; }

    [Range(0, 100, ErrorMessage = "Experience must be between 0 and 100 years.")]
    public int ExperienceYears { get; set; }

    [Range(0, 100000, ErrorMessage = "Appointment price must be between 0 and 100000.")]
    public decimal AppointmentPrice { get; set; }

    public string? ImageUrl { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Please select a specialization.")]
    public int SpecializationId { get; set; }
}
