using ECommerce.Core.Entities.Cart;

namespace ECommerce.Core.Interfaces.Repositories;

public interface ICartItemRepository : IGenericRepository<CartItem, int>
{
    Task<IReadOnlyList<CartItem>> GetItemsByCartIdAsync(int cartId, CancellationToken ct = default);
}
