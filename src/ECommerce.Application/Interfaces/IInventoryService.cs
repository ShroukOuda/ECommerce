using ECommerce.Application.DTO.Inventory;

namespace ECommerce.Application.Interfaces;

public interface IInventoryService
{
    Task<IEnumerable<GetInventoryHistoryDTO>> GetHistoryByProductIdAsync(Guid productId);
    Task<GetInventoryHistoryDTO> AddInventoryHistoryAsync(Guid productId, string userId, CreateInventoryHistoryDTO dto);
}
