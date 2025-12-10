using Microsoft.AspNetCore.Http;

namespace ECommerce.Application.DTO.Product;

public class AddProductDTO : ProductBaseDTO
{
    public int CategoryId { get; set; }
    public IFormFileCollection Photos { get; set; } 
}