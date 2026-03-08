using ECommerce.Core.Entities.Inventory;

namespace ECommerce.Core.Interfaces.Repositories;

public interface IInventoryHistoryRepository : IGenericRepository<InventoryHistory, int>
{
    Task<IReadOnlyList<InventoryHistory>> GetHistoryByProductIdAsync(int productId, CancellationToken ct = default);
}
