namespace ECommerce.Application.DTO.Order;

/// <summary>
/// Checkout request. Only the cart is needed: items, quantities, prices and currency come from the
/// server-side cart and settings. Legacy fields (currency, orderType, items, address IDs) that an
/// old client still sends are ignored by JSON binding.
/// </summary>
public class CreateOrderDTO
{
    public Guid CartId { get; set; }
}
