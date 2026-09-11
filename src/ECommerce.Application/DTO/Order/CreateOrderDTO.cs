namespace ECommerce.Application.DTO.Order;

public class CreateOrderDTO : OrderBaseDTO
{
    public Guid CartId { get; set; }
    public List<CreateOrderItemDTO> Items { get; set; } = new();
}
