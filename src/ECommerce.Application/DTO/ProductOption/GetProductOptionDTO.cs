namespace ECommerce.Application.DTO.ProductOption;

public class GetProductOptionDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayType { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string AttributeKey { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public decimal PriceValue { get; set; }
    public int SortOrder { get; set; }
    public Guid ProductId { get; set; }
    public List<GetProductOptionValueDTO> Values { get; set; } = new();
}
