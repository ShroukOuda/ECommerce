namespace ECommerce.Application.DTO.Order;

public class CreateOrderDTO : OrderBaseDTO
{
    public string UserId { get; set; } = string.Empty;
    public List<CreateOrderItemDTO> Items { get; set; } = new();
}
