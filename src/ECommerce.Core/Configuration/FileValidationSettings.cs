namespace ECommerce.Core.Configuration;

public class FileValidationSettings
{
    public const string SectionName = "FileValidation";
    public int MaxFileSizeInMB { get; set; }
    public List<string> AllowedExtensions { get; set; } = new List<string>();
    public ProductValidationSettings Product { get; set; } = new();
    public CategoryValidationSettings Category { get; set; } = new();
    public long MaxFileSizeInBytes => MaxFileSizeInMB * 1024 * 1024;
    
    public bool IsAllowedExtension(string extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
            return false;

        return AllowedExtensions.Contains(extension.ToLowerInvariant());
    }
}