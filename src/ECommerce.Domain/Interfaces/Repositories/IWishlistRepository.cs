using ECommerce.Domain.Entities.Wishlist;

namespace ECommerce.Domain.Interfaces.Repositories;

public interface IWishlistRepository : IGenericRepository<Wishlist, Guid>
{
    Task<IReadOnlyList<Wishlist>> GetWishlistByUserIdAsync(string userId, CancellationToken ct = default);
}
