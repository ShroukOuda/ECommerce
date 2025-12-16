namespace ECommerce.Infrastructure.Settings;

public class StorageFolderStructure
{
    public string Products { get; set; } = "Products";
    public string Categories { get; set; } = "Categories";
    
    public string CategoryBanners { get; set; } = "Banners";
    public string CategoryIcons { get; set; } = "Icons";
    public string CategoryThumbnails { get; set; } = "Thumbnails";
}