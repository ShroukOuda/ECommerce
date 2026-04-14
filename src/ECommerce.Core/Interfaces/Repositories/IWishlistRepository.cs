using ECommerce.Core.Entities.Wishlist;

namespace ECommerce.Core.Interfaces.Repositories;

public interface IWishlistRepository : IGenericRepository<Wishlist, Guid>
{
    Task<IReadOnlyList<Wishlist>> GetWishlistByUserIdAsync(string userId, CancellationToken ct = default);
}
