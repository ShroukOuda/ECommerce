using ECommerce.Core.Enums.Wishlist;

namespace ECommerce.Core.Entities.Wishlist;

public class Wishlist : BaseEntity<Guid>
{
    public WishlistStatus Status { get; set; } = WishlistStatus.Active;
    
    //FK
    public Guid ProductId { get; set; }
    public string UserId { get; set; }
    
    //Navigation Properties
    public virtual Product.Product? Product { get; set; }
    public virtual User.User? User { get; set; }
}