namespace ECommerce.Application.DTO.ProductVariant;

public class AddProductVariantDTO
{
    public string Sku { get; set; } = string.Empty;
    public string VariantName { get; set; } = string.Empty;
    public string? Size { get; set; }
    public string? Color { get; set; }
    public string? Material { get; set; }
    public decimal PriceAdjustment { get; set; }
    public int StockQuantity { get; set; }
    public int ProductId { get; set; }
    public List<int> OptionValueIds { get; set; } = new();
}
