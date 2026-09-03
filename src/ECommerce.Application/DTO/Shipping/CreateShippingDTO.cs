using ECommerce.Domain.Enums.Shipping;

namespace ECommerce.Application.DTO.Shipping;

public class CreateShippingDTO
{
    public Guid OrderId { get; set; }
    public Guid AddressId { get; set; }
    public ShippingMethod Method { get; set; } = ShippingMethod.Standard;
    public ShippingStatus Status { get; set; } = ShippingStatus.Pending;
    public decimal Cost { get; set; }
    public string? Carrier { get; set; }
    public DateTime? EstimatedDeliveryDate { get; set; }
    public string? TrackingNumber { get; set; }
}
