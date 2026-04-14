using ECommerce.Core.Entities.Cart;

namespace ECommerce.Core.Interfaces.Repositories;

public interface ICartItemRepository : IGenericRepository<CartItem, Guid>
{
    Task<IReadOnlyList<CartItem>> GetItemsByCartIdAsync(Guid cartId, CancellationToken ct = default);
}
