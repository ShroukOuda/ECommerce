using ECommerce.Domain.Enums.Shipping;

namespace ECommerce.Application.DTO.Shipping;

public class GetShippingDTO
{
    public Guid Id { get; set; }
    public string TrackingNumber { get; set; } = string.Empty;
    public ShippingMethod Method { get; set; } 
    public decimal Cost { get; set; }
    public ShippingStatus Status { get; set; }
    public DateTime? ShippedDate { get; set; }
    public DateTime? DeliveredDate { get; set; }
    public Guid OrderId { get; set; }
}
