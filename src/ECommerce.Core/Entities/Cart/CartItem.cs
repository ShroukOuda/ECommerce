namespace ECommerce.Core.Entities.Cart;

public class CartItem : BaseEntity<int>
{
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    
    //FK
    public int CartId { get; set; }
    public int ProductId { get; set; }
    public int VariantId { get; set; }
    
    //Navigation Properties
    public virtual Cart? Cart { get; set; }
    public virtual Product.Product? Product { get; set; }
    public virtual Product.ProductVariant? ProductVariant { get; set; }
}