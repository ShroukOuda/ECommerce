using ECommerce.Domain.Enums.Cart;

namespace ECommerce.Domain.Entities.Carts;

public class Cart : BaseEntity<Guid>
{
    public string GuestToken { get; set; } = string.Empty;
    public decimal DiscountAmount { get; set; }
    public DateTime ExpiresAt { get; set; }
    public CartStatus Status { get; set; } = CartStatus.Active;
    
    //FK
    public string? UserId { get; set; }
    
    //Navigation Properties
    public virtual User User { get; set; } = null!;
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}