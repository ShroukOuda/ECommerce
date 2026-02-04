namespace ECommerce.Core.Entities.Cart;

public class CartItemOption : BaseEntity<int>
{
    public string OptionName { get; set; } = string.Empty;
    public string OptionValue { get; set; } = string.Empty;
    public decimal PriceAdjustment { get; set; } 
    
    //FK
    public int ProductOptionId { get; set; } 
    public int CartItemId { get; set; }
    
    //Navigation Properties
    public virtual Product.ProductOption? ProductOption { get; set; }
    public virtual CartItem? CartItem { get; set; }
}