using Microsoft.AspNetCore.Http;

namespace ECommerce.Application.DTO.Photo;

public class UploadPhotoDTO
{
    public int EntityId { get; set; }
    public PhotoType Type { get; set; }
    public PhotoSubType? SubType { get; set; }
    public bool IsMain { get; set; }
    public string AltText { get; set; }
    public IFormFile File { get; set; } 
}