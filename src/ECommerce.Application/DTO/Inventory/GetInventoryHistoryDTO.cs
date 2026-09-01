using ECommerce.Domain.Enums.Inventory;

namespace ECommerce.Application.DTO.Inventory;

public class GetInventoryHistoryDTO
{
    public Guid Id { get; set; }
    public Guid? ProductVariantId { get; set; }
    public int QuantityChange { get; set; }
    public int NewQuantity { get; set; }
    public InventoryChangeType ChangeType { get; set; } 
    public string? Notes { get; set; }
    public string UserId { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
