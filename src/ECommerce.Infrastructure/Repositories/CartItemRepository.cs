using ECommerce.Domain.Entities.Cart;

namespace ECommerce.Infrastructure.Repositories;

public class CartItemRepository : GenericRepository<CartItem, Guid>, ICartItemRepository
{
    public CartItemRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<CartItem>> GetItemsByCartIdAsync(Guid cartId, CancellationToken ct = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(ci => ci.CartId == cartId)
            .Include(ci => ci.Product)
            .Include(ci => ci.CartItemOptions)
            .ToListAsync(ct);
    }
}
