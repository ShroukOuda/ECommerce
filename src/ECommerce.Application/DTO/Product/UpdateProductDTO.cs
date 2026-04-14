using Microsoft.AspNetCore.Http;

namespace ECommerce.Application.DTO.Product;

public class UpdateProductDTO : ProductBaseDTO
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
  
}