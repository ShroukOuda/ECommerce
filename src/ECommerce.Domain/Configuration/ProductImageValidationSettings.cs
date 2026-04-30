namespace ECommerce.Domain.Configuration;

public class ProductImageValidationSettings
{
    public int MaxTotalPhotos { get; set; } = 10;
    public int MaxMainPhotos { get; set; } = 1;
    public int MaxFileSizeInMB { get; set; } = 5;
    public long MaxFileSizeInBytes => MaxFileSizeInMB * 1024 * 1024;
}