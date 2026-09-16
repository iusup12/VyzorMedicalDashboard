namespace Vyzor.Application.DTO.AppointmentCart;

public class AppointmentCartCheckoutResultDTO
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public int AppointmentsCreated { get; set; }

    public decimal SubTotal { get; set; }

    public decimal DiscountPercent { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal Total { get; set; }
}