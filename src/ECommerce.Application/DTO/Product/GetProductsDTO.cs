namespace ECommerce.Application.DTO.Product;

public class GetProductsDTO : ProductBaseDTO
{
    public Guid Id { get; set; }
    public string CategoryName { get; set; } = null!;
    public string BrandName { get; set; } = null!;
    public string ProductMainImageUrl { get; set; } = null!;
   
}