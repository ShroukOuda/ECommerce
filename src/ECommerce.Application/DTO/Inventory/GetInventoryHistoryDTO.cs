namespace ECommerce.Application.DTO.Inventory;

public class GetInventoryHistoryDTO
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid? ProductVariantId { get; set; }
    public int QuantityChange { get; set; }
    public int NewQuantity { get; set; }
    public string ChangeType { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
