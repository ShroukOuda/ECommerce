using Microsoft.AspNetCore.Http;

namespace ECommerce.Application.DTO.Product;

public class UpdateProductDTO : ProductBaseDTO
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public IFormFileCollection? NewPhotos { get; set; }
    public List<int>? PhotosToDelete { get; set; }
}