using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Wishlists;

namespace ECommerce.Application.Specifications.Wishlists;

public class WishlistSpecification : BaseSpecification<Wishlist, Guid>
{
    public WishlistSpecification(Guid wishlistId)
        : base(w => w.Id == wishlistId)
    {
        
    }

    public WishlistSpecification(Guid productId, string userId)
        : base(w => w.ProductId == productId && w.UserId == userId)
    {
        
    }
}
