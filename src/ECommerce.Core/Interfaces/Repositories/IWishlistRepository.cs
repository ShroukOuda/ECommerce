using ECommerce.Core.Entities.Wishlist;

namespace ECommerce.Core.Interfaces.Repositories;

public interface IWishlistRepository : IGenericRepository<Wishlist, int>
{
    Task<IReadOnlyList<Wishlist>> GetWishlistByUserIdAsync(string userId, CancellationToken ct = default);
}
