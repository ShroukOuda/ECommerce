namespace ECommerce.Application.DTO.ProductOption;

public class GetProductOptionValueDTO
{
    public Guid Id { get; set; }
    public string Value { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public decimal PriceValue { get; set; }
    public bool IsDefault { get; set; }
    public int SortOrder { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public Guid OptionId { get; set; }
}
