
using Vyzor.Domain.Common;

namespace Vyzor.Domain.Entities;

public class AppointmentCartItem : Entity
{
    public string UserId { get; set; } = string.Empty;

    public int DoctorId { get; set; }

    public Doctor? Doctor { get; set; }

    public DateTime AppointmentDate { get; set; }

    public decimal Price { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

