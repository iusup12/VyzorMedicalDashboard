
namespace Vyzor.Application.DTO.AppointmentCart;

public class AppointmentCartItemDTO
{
    public int Id { get; set; }

    public int DoctorId { get; set; }

    public string DoctorName { get; set; } = string.Empty;

    public string? DoctorImageUrl { get; set; }

    public string SpecializationName { get; set; } = string.Empty;

    public DateTime AppointmentDate { get; set; }

    public decimal Price { get; set; }
}

