using ECommerce.Domain.Enums.Inventory;

namespace ECommerce.Application.DTO.Product;

public class GetProductDetailsDTO : ProductBaseDTO
{
    public Guid Id { get; set; }
    public string CategoryName { get; set; } = null!;
    public string BrandName { get; set; } = null!;
    public List<string> ImageUrls { get; set; } = new List<string>();
    public StockStatus StockStatus { get; set; }
   
}