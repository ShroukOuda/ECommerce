using System.Text.Json.Serialization;
using ECommerce.Application.DTO.Pagination;

namespace ECommerce.Application.DTO.Product;

public class ProductSpecParams : PaginationParams
{
    private string? _search;

    public string? Search
    {
        get => _search;
        set => _search = value?.Trim().ToLower();
    }
    public Guid? CategoryId { get; set; }
    public Guid? BrandId { get; set; }
    public decimal? MaxPrice { get; set; } = null;
    public decimal? MinPrice { get; set; } = null;
    public bool? IsFeatured { get; set; } = null;
    public bool? IsBestSeller { get; set; } = null;
    public bool? IsHotDeal { get; set; } = null;
    public bool? IsNewArrival { get; set; } = null;
    public bool? IsTopRated { get; set; } = null;
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ProductSortBy SortBy { get; set; } = ProductSortBy.Newest;
    
    public void ValidatePrices()
    {
        if (MinPrice.HasValue && MaxPrice.HasValue && MinPrice > MaxPrice)
            throw new ArgumentException("MinPrice cannot be greater than MaxPrice.");
    }

}
