namespace ECommerce.Application.Configuration;

public class FileValidationSettings
{
    public const string SectionName = "FileValidation";
    public int MaxFileSizeInMB { get; set; } = 10;
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