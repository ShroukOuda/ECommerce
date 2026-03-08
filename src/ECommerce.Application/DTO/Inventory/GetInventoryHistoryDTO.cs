namespace ECommerce.Application.DTO.Inventory;

public class GetInventoryHistoryDTO
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int? ProductVariantId { get; set; }
    public int QuantityChange { get; set; }
    public int NewQuantity { get; set; }
    public string ChangeType { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
