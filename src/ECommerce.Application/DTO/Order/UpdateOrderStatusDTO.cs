namespace ECommerce.Application.DTO.Order;

public class UpdateOrderStatusDTO
{
    public string OrderStatus { get; set; } = string.Empty;

    // Optional note recorded in the order status history.
    public string? Reason { get; set; }
}
