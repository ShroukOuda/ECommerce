using ECommerce.Domain.Enums.Shipping;

namespace ECommerce.Application.DTO.Shipping;

public class UpdateShippingDTO
{
    public string? TrackingNumber { get; set; }

    public string? Carrier { get; set; }

    public decimal? Cost { get; set; }
    public ShippingMethod Method { get; set; }
    public ShippingStatus Status { get; set; }

    public DateTime? EstimatedDeliveryDate { get; set; }
}