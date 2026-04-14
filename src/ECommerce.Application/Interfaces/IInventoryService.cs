using ECommerce.Application.DTO.Inventory;

namespace ECommerce.Application.Interfaces;

public interface IInventoryService
{
    Task<IEnumerable<GetInventoryHistoryDTO>> GetHistoryByProductIdAsync(Guid productId, CancellationToken ct = default);
    Task AddInventoryHistoryAsync(CreateInventoryHistoryDTO dto, CancellationToken ct = default);
}
