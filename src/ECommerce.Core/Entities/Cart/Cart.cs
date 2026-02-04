namespace ECommerce.Core.Entities.Cart;

public class Cart : BaseEntity<int>
{
    public string GuestToken { get; set; } = string.Empty;
    public decimal DiscountAmount { get; set; }
    public DateTime ExpiresAt { get; set; }
    
    //Navigation Properties
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public virtual ICollection<CartItemOption> CartItemOptions { get; set; } = new List<CartItemOption>();
}