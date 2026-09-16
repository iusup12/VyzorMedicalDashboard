namespace Vyzor.Application.DTO.AppointmentCart;

public class AppointmentCartSummaryDTO
{
    public int ItemsCount { get; set; }

    public decimal SubTotal { get; set; }

    public decimal DiscountPercent { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal Total { get; set; }

    public string? SubscriptionName { get; set; }
}