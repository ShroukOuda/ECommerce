using System.Text.Json.Serialization;

namespace ECommerce.Application.DTO.Product;

public class ProductParams
{
    public string? Search { get; set; }
    public Guid? CategoryId { get; set; }
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