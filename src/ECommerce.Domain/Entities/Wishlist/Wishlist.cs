using ECommerce.Domain.Common;
using ECommerce.Domain.Enums.Wishlist;

namespace ECommerce.Domain.Entities.Wishlist;

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