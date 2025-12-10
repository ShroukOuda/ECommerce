namespace ECommerce.Application.DTO.Product;

public class ProductBaseDTO
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string SKU { get; set; }
    
}