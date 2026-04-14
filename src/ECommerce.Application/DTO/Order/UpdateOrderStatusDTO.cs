namespace ECommerce.Application.DTO.Order;

public class UpdateOrderStatusDTO
{
    public Guid Id { get; set; }
    public string OrderStatus { get; set; } = string.Empty;
}
