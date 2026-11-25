namespace ECommerce.Application.DTO.Order;

/// <summary>
/// Checkout request. Items, quantities, prices and currency come from the server-side cart and settings.
/// Legacy request fields (currency, orderType, items) are ignored by JSON binding if an old client still sends them.
/// </summary>
public class CreateOrderDTO
{
    public Guid CartId { get; set; }
    public Guid? ShippingAddressId { get; set; }
    public Guid? BillingAddressId { get; set; }
}
