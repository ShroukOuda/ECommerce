using ECommerce.Domain.Enums.Inventory;

namespace ECommerce.Application.DTO.Inventory;

public class CreateInventoryHistoryDTO
{
    public Guid? ProductVariantId { get; set; }
    public int QuantityChange { get; set; }
    public InventoryChangeType ChangeType { get; set; } 
    public string? Notes { get; set; }

}
