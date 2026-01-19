using Microsoft.AspNetCore.Http;

namespace ECommerce.Application.DTO.Common;

public class UploadImageDTO
{
    public IFormFile File { get; set; } = null!;
    public string? AltText { get; set; }
}