namespace ECommerce.Core.Configuration;

public class CategoryValidationSettings
{
    public int IconMaxSizeKB { get; set; }
    public int BannerMaxSizeMB { get; set; } 
    public int ThumbnailMaxSizeMB { get; set; } 
    public int MaxImagesPerCategory { get; set; }
    public List<string> IconAllowedExtensions { get; set; } = new List<string>();
    public long IconMaxSizeInBytes => IconMaxSizeKB * 1024;
    public long BannerMaxSizeInBytes => BannerMaxSizeMB * 1024 * 1024;
    public long ThumbnailMaxSizeInBytes => ThumbnailMaxSizeMB * 1024 * 1024;
}