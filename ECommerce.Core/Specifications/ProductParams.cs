using System.Text.Json.Serialization;

namespace ECommerce.Core.Specifications;

public class ProductParams : PaginationParams
{
    public string? Search { get; set; }
    public int? CategoryId { get; set; }
    public decimal? MaxPrice { get; set; } = null;
    public decimal? MinPrice { get; set; } = null;
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ProductSortBy SortBy { get; set; } = ProductSortBy.NameAsc;
    
    public void ValidatePrices()
    {
        if (MinPrice.HasValue && MaxPrice.HasValue && MinPrice > MaxPrice)
            throw new ArgumentException("MinPrice cannot be greater than MaxPrice.");
    }

}