namespace Vyzor.Application.DTO.AppointmentCart;

public class AppointmentCartPaymentResult
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public int Count { get; set; }

    public decimal Subtotal { get; set; }

    public decimal Discount { get; set; }

    public decimal Total { get; set; }
}