namespace ECommerce.Application.Configuration;

public class FileValidationSettings
{
    public const string SectionName = "FileValidation";
    public int MaxFileSizeInMB { get; set; } = 10;
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
    public List<string> AllowedExtensions { get; set; } = new()
    {
        "jpg", "jpeg", "png"
    };
    
    public ProductImageValidationSettings ProductImage { get; set; } = new();
    public CategoryImageValidationSettings CategoryImage { get; set; } = new();
    public long MaxFileSizeInBytes => MaxFileSizeInMB * 1024 * 1024;
    
    public bool IsAllowedExtension(string extension) => 
        !string.IsNullOrWhiteSpace(extension) &&
        AllowedExtensions.Contains(extension.ToLowerInvariant());
}