namespace ECommerce.Core.Common;

public class BaseImage 
{
    public int Id { get; set; }
    public string ImageUrl { get; set; }
    public string AltText { get; set; }
    public DateTime UploadedAt { get; set; }
}