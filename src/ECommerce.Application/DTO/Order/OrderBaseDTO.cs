namespace ECommerce.Application.DTO.Order;

public class OrderBaseDTO
{
    public string OrderType { get; set; } = "Standard";
    public string Currency { get; set; } = "USD";
    public Guid? ShippingAddressId { get; set; }
    public Guid? BillingAddressId { get; set; }
}
