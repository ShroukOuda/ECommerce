namespace E_Commerece.Infrastructure.Settings;

public class FileSettings
{
    public string ImagesPath { get; set; } = string.Empty;
    public long MaxFileSizeInBytes { get; set; }
    public int MaxFileSizeInMB { get; set; }
    public List<string> AllowedExtensions { get; set; } = new List<string>();
}