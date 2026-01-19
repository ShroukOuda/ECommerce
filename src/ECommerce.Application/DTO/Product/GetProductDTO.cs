using ECommerce.Application.DTO.ProductImages;

namespace ECommerce.Application.DTO.Product;

public class GetProductDTO : ProductBaseDTO
{
    public int Id { get; set; }
    public string CategoryName { get; set; }
    public List<ProductImageDTO> Photos { get; set; }
}