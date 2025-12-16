using Microsoft.AspNetCore.Http;

namespace ECommerce.Application.DTO.Photo;

public class UploadPhotosDTO
{
    public int EntityId { get; set; }
    public PhotoType Type { get; set; }
    public bool MakeFirstAsMain { get; set; }
    public List<string> AltTexts { get; set; }
    public IFormFileCollection Files { get; set; } 
}