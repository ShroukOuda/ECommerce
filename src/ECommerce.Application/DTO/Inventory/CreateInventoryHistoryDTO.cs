namespace ECommerce.Application.DTO.Inventory;

public class CreateInventoryHistoryDTO
{
    public int ProductId { get; set; }
    public int? ProductVariantId { get; set; }
    public int QuantityChange { get; set; }
    public int NewQuantity { get; set; }
    public string ChangeType { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string? UserId { get; set; }
}
