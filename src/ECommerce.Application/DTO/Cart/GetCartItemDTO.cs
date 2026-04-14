namespace ECommerce.Application.DTO.Cart;

public class GetCartItemDTO
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public Guid VariantId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
