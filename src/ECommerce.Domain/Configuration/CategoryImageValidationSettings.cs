namespace ECommerce.Domain.Configuration;

public class CategoryImageValidationSettings
{
    public int MaxIcons { get; set; } = 1;
    public int MaxBanners { get; set; } = 1;
    public int MaxThumbnails { get; set; } = 1;
    public int IconMaxSizeKB { get; set; } = 100;
    public int BannerMaxSizeMB { get; set; } = 5;
    public int ThumbnailMaxSizeMB { get; set; } = 2;
    public List<string> IconAllowedExtensions { get; set; } = new() { ".svg", ".png"};
    public long IconMaxSizeInBytes => IconMaxSizeKB * 1024;
    public long BannerMaxSizeInBytes => BannerMaxSizeMB * 1024 * 1024;
    public long ThumbnailMaxSizeInBytes => ThumbnailMaxSizeMB * 1024 * 1024;
}