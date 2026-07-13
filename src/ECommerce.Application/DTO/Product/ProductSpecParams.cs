using System.Text.Json.Serialization;
using ECommerce.Application.DTO.Pagination;

namespace ECommerce.Application.DTO.Product;

public class ProductSpecParams : PaginationParams
{
    public string? Search { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? BrandId { get; set; }
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