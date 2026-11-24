namespace ECommerce.Application.DTO.Order;

public class CreateOrderItemDTO
{
    public Guid ProductId { get; set; }
    public Guid? ProductVariantId { get; set; }
    public Dictionary<string, string> VariantAttributes { get; set; } = new();
    public int Quantity { get; set; }
    public List<CreateOrderItemOptionDTO> Options { get; set; } = new();
}
