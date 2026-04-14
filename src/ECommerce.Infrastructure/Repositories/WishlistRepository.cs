using ECommerce.Core.Entities.Wishlist;

namespace ECommerce.Infrastructure.Repositories;

public class WishlistRepository : GenericRepository<Wishlist, Guid>, IWishlistRepository
{
    public WishlistRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Wishlist>> GetWishlistByUserIdAsync(string userId, CancellationToken ct = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(w => w.UserId == userId)
            .Include(w => w.Product)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync(ct);
    }
}
