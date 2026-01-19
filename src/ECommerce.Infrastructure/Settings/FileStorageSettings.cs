namespace ECommerce.Infrastructure.Settings;

public class FileStorageSettings
{
    public const string SectionName = "FileStorage";
    public string BasePath { get; set; } = "Images";
    public FileNamingStrategy NamingStrategy { get; set; } 
    public bool UseEntitySubfolders { get; set; }
    public bool UseDateSubfolders { get; set; }
}