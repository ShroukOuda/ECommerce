namespace ECommerce.Application.DTO.ProductOption;

public class UpdateProductOptionDTO
{
    public string Name { get; set; } = string.Empty;
    public string DisplayType { get; set; } = "Dropdown";
    public string Type { get; set; } = "VariantSelector";
    public string AttributeKey { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public decimal PriceValue { get; set; }
    public int SortOrder { get; set; }
}
