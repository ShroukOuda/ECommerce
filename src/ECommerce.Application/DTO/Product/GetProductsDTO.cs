namespace ECommerce.Application.DTO.Product;

public class GetProductsDTO : ProductBaseDTO
{
    public Guid Id { get; set; }
    public string CategoryName { get; set; } = null!;
    public string BrandName { get; set; } = null!;
    public bool IsOnSale { get; set; }
    public decimal DiscountPercentage { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsNewArrival { get; set; }
    public bool IsHotDeal { get; set; }
    public bool IsTopRated { get; set; }
    public bool IsBestSeller { get; set; }
    public decimal AverageRating { get; set; }
    public string ProductMainImageUrl { get; set; } = null!;
   
}