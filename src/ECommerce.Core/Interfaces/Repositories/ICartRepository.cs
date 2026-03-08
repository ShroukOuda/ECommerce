using ECommerce.Core.Entities.Cart;

namespace ECommerce.Core.Interfaces.Repositories;

public interface ICartRepository : IGenericRepository<Cart, int>
{
    Task<Cart?> GetCartWithItemsAsync(int cartId, CancellationToken ct = default);
    Task<Cart?> GetActiveCartByUserIdAsync(string userId, CancellationToken ct = default);
}
