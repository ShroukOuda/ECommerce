namespace ECommerce.Application.DTO.Order;

/// <summary>
/// Checkout request. Items, quantities, prices, currency and order type come from the server-side cart and settings.
/// Client input: the cart, plus optional saved shipping and billing address IDs (checked to belong to the user).
/// Legacy fields (currency, orderType, items) that an old client still sends are ignored by JSON binding.
/// </summary>
public class CreateOrderDTO
{
    public Guid CartId { get; set; }
    public Guid? ShippingAddressId { get; set; }
    public Guid? BillingAddressId { get; set; }
}
