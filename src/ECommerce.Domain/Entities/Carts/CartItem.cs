

namespace ECommerce.Domain.Entities.Carts;

public class CartItem : BaseEntity<Guid>
{
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    
    //FK
    public Guid CartId { get; set; }
    public Guid ProductId { get; set; }
    public Guid VariantId { get; set; }
    
    //Navigation Properties
    public  Cart Cart { get; set; } = null!;
    public  Product Product { get; set; } = null!;
    public  ProductVariant ProductVariant { get; set; } = null!;
    public  ICollection<CartItemOption> CartItemOptions { get; set; } = new List<CartItemOption>();
}