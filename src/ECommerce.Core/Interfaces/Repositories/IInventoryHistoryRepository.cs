using ECommerce.Core.Entities.Inventory;

namespace ECommerce.Core.Interfaces.Repositories;

public interface IInventoryHistoryRepository : IGenericRepository<InventoryHistory, Guid>
{
    Task<IReadOnlyList<InventoryHistory>> GetHistoryByProductIdAsync(Guid productId, CancellationToken ct = default);
}
