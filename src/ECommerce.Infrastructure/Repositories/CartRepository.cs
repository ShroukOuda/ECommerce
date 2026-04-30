using ECommerce.Domain.Entities.Cart;

namespace ECommerce.Infrastructure.Repositories;

public class CartRepository : GenericRepository<Cart, Guid>, ICartRepository
{
    private readonly AppDbContext _context;

    public CartRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Cart?> GetCartWithItemsAsync(Guid cartId, CancellationToken ct = default)
    {
        return await _context.Carts
            .Include(c => c.CartItems).ThenInclude(ci => ci.CartItemOptions)
            .Include(c => c.CartItems).ThenInclude(ci => ci.Product)
            .Include(c => c.CartItems).ThenInclude(ci => ci.ProductVariant)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == cartId, ct);
    }

    public async Task<Cart?> GetActiveCartByUserIdAsync(string userId, CancellationToken ct = default)
    {
        return await _context.Carts
            .Include(c => c.CartItems).ThenInclude(ci => ci.Product)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.UserId == userId && c.Status == ECommerce.Domain.Enums.Cart.CartStatus.Active, ct);
    }
}
