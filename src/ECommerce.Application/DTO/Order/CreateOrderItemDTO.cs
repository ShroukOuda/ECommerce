namespace ECommerce.Application.DTO.Order;

public class CreateOrderItemDTO
{
    public Guid ProductId { get; set; }
    public Guid ProductVariantId { get; set; }
    public int Quantity { get; set; }
}
