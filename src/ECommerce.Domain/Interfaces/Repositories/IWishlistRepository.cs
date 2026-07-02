using ECommerce.Domain.Entities.Wishlists;

namespace ECommerce.Domain.Interfaces.Repositories;

public interface IWishlistRepository : IGenericRepository<Wishlist, Guid>
{
    Task<IReadOnlyList<Wishlist>> GetWishlistByUserIdAsync(string userId, CancellationToken ct = default);
}
