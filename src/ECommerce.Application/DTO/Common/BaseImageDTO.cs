namespace ECommerce.Application.DTO.Common;

public class BaseImageDTO
{
    public Guid Id { get; set; }
    public string ImageUrl { get; set; } 
    public string AltText { get; set; }
    public DateTime UploadedAt { get; set; }
}