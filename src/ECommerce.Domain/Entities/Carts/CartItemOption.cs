

namespace ECommerce.Domain.Entities.Carts;

public class CartItemOption : BaseEntity<Guid>
{
    public string OptionName { get; set; } = string.Empty;
    public string OptionValue { get; set; } = string.Empty;
    public decimal PriceAdjustment { get; set; } 
    
    //FK
    public Guid ProductOptionId { get; set; } 
    public Guid CartItemId { get; set; }
    
    //Navigation Properties
    public  ProductOption ProductOption { get; set; } = null!;
    public  CartItem CartItem { get; set; } = null!;
}