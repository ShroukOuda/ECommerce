namespace ECommerce.Application.DTO.Cart;

public class AddCartItemDTO
{
    public Guid CartId { get; set; }
    public Guid ProductId { get; set; }
    public Guid VariantId { get; set; }
    public int Quantity { get; set; }
}
