using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using ECommerce.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace ECommerce.Infrastructure.Services;


public class ImageManagementService : IImageManagementService
{
    private readonly IFileProvider _fileProvider;
    private readonly FileSettings _fileSettings;

    public ImageManagementService(IFileProvider fileProvider, IOptions<FileSettings> fileSettings)
    {
        _fileProvider = fileProvider;
        _fileSettings = fileSettings.Value;
    }
    public async Task<List<string>> AddImageAsync(IFormFileCollection files, string folderName)
    {
        List<string> SaveImageSrc =  new List<string>();
        string root = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        string imagesPath = Path.Combine(root, _fileSettings.ImagesPath, folderName);
        

        if (!Directory.Exists(imagesPath))
        {
            Directory.CreateDirectory(imagesPath);
        }

        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (!_fileSettings.AllowedExtensions.Contains(extension))
                {
                    throw new Exception("Invalid file format. Allowed: " +
                                        string.Join(", ", _fileSettings.AllowedExtensions));
                }

                if (file.Length > _fileSettings.MaxFileSizeInBytes)
                {
                    throw new Exception($"Filer too large. Max Size: {_fileSettings.MaxFileSizeInMB}MB");
                }
                var newName = $"{Guid.NewGuid()}{extension}";
                var fullPath = Path.Combine(imagesPath, newName);

                using (FileStream fs = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(fs);
                }

                string ImageSrc = $"{_fileSettings.ImagesPath}/{folderName}/{newName}";
                SaveImageSrc.Add(ImageSrc);
            }
        }
        
        return SaveImageSrc;
    }

    public async Task DeleteImageFile(string src)
    {
        src = src.TrimStart('/');

        string fullPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            src
        );

        if (File.Exists(fullPath))
            await Task.Run(() => File.Delete(fullPath));
    }


    public async Task DeleteImagesFolder(string folderName)
    {
        folderName = folderName.TrimStart('/');

        string folderPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            _fileSettings.ImagesPath,
            folderName
        );
        
        if (Directory.Exists(folderPath))
            await Task.Run(() => Directory.Delete(folderPath, true));
    }

}