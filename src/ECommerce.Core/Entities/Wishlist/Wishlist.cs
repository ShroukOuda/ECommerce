using ECommerce.Core.Enums.Wishlist;

namespace ECommerce.Core.Entities.Wishlist;

public class Wishlist : BaseEntity<int>
{
    public WishlistStatus Status { get; set; } = WishlistStatus.Active;
    
    //FK
    public int ProductId { get; set; }
    public int UserId { get; set; }
    
    //Navigation Properties
    public virtual Product.Product? Product { get; set; }
    public virtual User.User? User { get; set; }
}