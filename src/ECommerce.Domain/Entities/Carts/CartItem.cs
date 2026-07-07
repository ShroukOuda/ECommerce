

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
    public virtual Cart Cart { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
    public virtual ProductVariant ProductVariant { get; set; } = null!;
    public virtual ICollection<CartItemOption> CartItemOptions { get; set; } = new List<CartItemOption>();
}