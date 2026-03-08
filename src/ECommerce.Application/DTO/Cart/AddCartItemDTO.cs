namespace ECommerce.Application.DTO.Cart;

public class AddCartItemDTO
{
    public int CartId { get; set; }
    public int ProductId { get; set; }
    public int VariantId { get; set; }
    public int Quantity { get; set; }
}
