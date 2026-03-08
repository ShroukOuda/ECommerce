namespace ECommerce.Application.DTO.Shipping;

public class GetShippingDTO
{
    public int Id { get; set; }
    public string TrackingNumber { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? ShippedDate { get; set; }
    public DateTime? DeliveredDate { get; set; }
    public int OrderId { get; set; }
}
