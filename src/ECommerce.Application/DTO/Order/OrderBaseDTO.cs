namespace ECommerce.Application.DTO.Order;

public class OrderBaseDTO
{
    public string OrderType { get; set; } = "Standard";
    public string Currency { get; set; } = "USD";
    public int? ShippingAddressId { get; set; }
    public int? BillingAddressId { get; set; }
}
