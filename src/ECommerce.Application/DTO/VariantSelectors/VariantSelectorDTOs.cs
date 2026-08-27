using ECommerce.Application.DTO.Product;
using ECommerce.Application.DTO.ProductVariant;

namespace ECommerce.Application.DTO.VariantSelectors;

public class VariantSelectorOptionValueDTO
{
    public Guid ValueId { get; set; }
    public string Label { get; set; } = string.Empty;
    public decimal PriceAdjustment { get; set; }
    public bool IsAvailable { get; set; }
    public string? HexCode { get; set; }
}

public class VariantSelectorOptionDTO
{
    public Guid OptionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayType { get; set; } = "dropdown";
    public bool IsRequired { get; set; }
    public int SortOrder { get; set; }
    public IReadOnlyList<VariantSelectorOptionValueDTO> Values { get; set; } = Array.Empty<VariantSelectorOptionValueDTO>();
}

public class VariantSelectorsResponseDTO
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public IReadOnlyList<VariantSelectorOptionDTO> VariantSelectors { get; set; } = Array.Empty<VariantSelectorOptionDTO>();
}

public class FindVariantRequestDTO
{
    public IReadOnlyList<Guid> OptionValueIds { get; set; } = Array.Empty<Guid>();
}

public class FindVariantResponseDTO
{
    public GetProductsDTO? Product { get; set; }
    public GetProductVariantDTO? Variant { get; set; }
}
