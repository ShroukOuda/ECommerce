using ECommerce.Domain.Entities.Inventory;

namespace ECommerce.Domain.Interfaces.Repositories;

public interface IInventoryHistoryRepository : IGenericRepository<InventoryHistory, Guid>
{
    Task<IReadOnlyList<InventoryHistory>> GetHistoryByProductIdAsync(Guid productId, CancellationToken ct = default);
}
