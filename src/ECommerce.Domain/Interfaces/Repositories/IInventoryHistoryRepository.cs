using ECommerce.Domain.Entities.Inventories;

namespace ECommerce.Domain.Interfaces.Repositories;

public interface IInventoryHistoryRepository : IGenericRepository<InventoryHistory, Guid>
{
    Task<IReadOnlyList<InventoryHistory>> GetHistoryByProductIdAsync(Guid productId, CancellationToken ct = default);
}
