namespace ECommerce.Core.Configuration;

public class ProductValidationSettings
{
    public int MaxImagesPerProduct { get; set; }
    public int MaxFileSizeInMB { get; set; } 

    public long MaxFileSizeInBytes => MaxFileSizeInMB * 1024 * 1024;
}