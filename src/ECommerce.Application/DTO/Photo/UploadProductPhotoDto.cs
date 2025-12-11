using Microsoft.AspNetCore.Http;

namespace ECommerce.Application.DTO.Photo;

public class UploadProductPhotoDto
{
    public IFormFileCollection Photos { get; set; } 
}