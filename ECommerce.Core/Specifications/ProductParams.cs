using System.Text.Json.Serialization;

namespace E_Commerece.Core.Specifications;

public class ProductParams : PaginationParams
{
    public string? Search { get; set; }
    public int? CategoryId { get; set; }
    public float? MaxPrice { get; set; } = null;
    public float? MinPrice { get; set; } = null;
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ProductSortBy SortBy { get; set; } = ProductSortBy.NameAsc;
    
    public void ValidatePrices()
    {
        if (MinPrice.HasValue && MaxPrice.HasValue && MinPrice > MaxPrice)
            throw new ArgumentException("MinPrice cannot be greater than MaxPrice.");
    }

}