namespace ECommerce.Application.DTO.Order;

public class CreateOrderItemDTO
{
    public int ProductId { get; set; }
    public int ProductVariantId { get; set; }
    public int Quantity { get; set; }
}
