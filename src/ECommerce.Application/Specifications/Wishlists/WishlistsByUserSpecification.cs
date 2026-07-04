using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Wishlists;

namespace ECommerce.Application.Specifications.Wishlists;

public class WishlistsByUserSpecification : BaseSpecification<Wishlist, Guid>
{
    public WishlistsByUserSpecification(string userId)
        : base(w => w.UserId == userId)
    {
        AddInclude(w => w.Product);
        AddOrderByDescending(w => w.CreatedAt);
        AsNoTracking();
    }
}
