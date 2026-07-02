using ECommerce.Domain.Entities.Carts;

namespace ECommerce.Domain.Interfaces.Repositories;

public interface ICartRepository : IGenericRepository<Cart, Guid>
{
    Task<Cart?> GetCartWithItemsAsync(Guid cartId, CancellationToken ct = default);
    Task<Cart?> GetActiveCartByUserIdAsync(string userId, CancellationToken ct = default);
}
