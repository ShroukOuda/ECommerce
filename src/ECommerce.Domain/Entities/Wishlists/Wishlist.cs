
using ECommerce.Domain.Enums.Wishlist;

namespace ECommerce.Domain.Entities.Wishlists;

public class Wishlist : BaseEntity<Guid>
{
    public WishlistStatus Status { get; set; } = WishlistStatus.Active;
    
    //FK
    public Guid ProductId { get; set; }
    public string UserId { get; set; } = null!;
    
    //Navigation Properties
    public  Product Product { get; set; } = null!;
    public  User User { get; set; } = null!;
}