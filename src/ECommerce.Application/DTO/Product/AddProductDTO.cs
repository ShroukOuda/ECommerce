using Microsoft.AspNetCore.Http;

namespace ECommerce.Application.DTO.Product;

public class AddProductDTO : ProductBaseDTO
{
    public Guid CategoryId { get; set; }
    public Guid BrandId { get; set; }
    
}