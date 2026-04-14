namespace ECommerce.Core.Entities.Cart;

public class CartItemOption : BaseEntity<Guid>
{
    public string OptionName { get; set; } = string.Empty;
    public string OptionValue { get; set; } = string.Empty;
    public decimal PriceAdjustment { get; set; } 
    
    //FK
    public Guid ProductOptionId { get; set; } 
    public Guid CartItemId { get; set; }
    
    //Navigation Properties
    public virtual Product.ProductOption? ProductOption { get; set; }
    public virtual CartItem? CartItem { get; set; }
}