namespace E_Commerece.Infrastructure.Settings;

public class FileSettings
{
    public const string ImagesPath = "Images/Products";
    public const string AllowedExtensions = ".jpg,.jpeg,.png";
    public const int MaxFileSizeInMB = 5;
    public const int MaxFileSizeInBytes = MaxFileSizeInMB * 1024 * 1024;
}