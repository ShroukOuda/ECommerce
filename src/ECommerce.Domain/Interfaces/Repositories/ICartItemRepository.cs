using ECommerce.Domain.Entities.Carts;

namespace ECommerce.Domain.Interfaces.Repositories;

public interface ICartItemRepository : IGenericRepository<CartItem, Guid>
{
    Task<IReadOnlyList<CartItem>> GetItemsByCartIdAsync(Guid cartId, CancellationToken ct = default);
}
