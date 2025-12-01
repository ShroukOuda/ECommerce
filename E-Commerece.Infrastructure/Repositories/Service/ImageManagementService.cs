using E_Commerece.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using E_Commerece.Infrastructure.Settings;

namespace E_Commerece.Infrastructure.Repositories.Service;


public class ImageManagementService : IImageManagementService
{
    private readonly IFileProvider _fileProvider;

    public ImageManagementService(IFileProvider fileProvider)
    {
        _fileProvider = fileProvider;
    }
    public async Task<List<string>> AddImageAsync(IFormFileCollection files, string productFolder)
    {
        List<string> SaveImageSrc =  new List<string>();
        string root = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        string imagesPath = Path.Combine(root, FileSettings.ImagesPath, productFolder);
        

        if (!Directory.Exists(imagesPath))
        {
            Directory.CreateDirectory(imagesPath);
        }

        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (!FileSettings.AllowedExtensions.Contains(extension))
                {
                    throw new Exception("Invalid file format. Allowed: " + FileSettings.AllowedExtensions);
                }

                if (file.Length > FileSettings.MaxFileSizeInBytes)
                {
                    throw new Exception($"Filer too large. Max Size: {FileSettings.MaxFileSizeInMB}MB");
                }
                var newName = $"{Guid.NewGuid()}{extension}";
                var fullPath = Path.Combine(imagesPath, newName);

                using (FileStream fs = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(fs);
                }

                string ImageSrc = $"{FileSettings.ImagesPath}/{productFolder}/{newName}";
                SaveImageSrc.Add(ImageSrc);
            }
        }
        
        return SaveImageSrc;
    }

    public void DeleteImageFile(string src)
    {
        src = src.TrimStart('/');

        string fullPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            src
        );

        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }


    public void DeleteImagesFolder(string folderName)
    {
        folderName = folderName.TrimStart('/');

        string folderPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            FileSettings.ImagesPath,
            folderName
        );
        
        if (Directory.Exists(folderPath))
            Directory.Delete(folderPath, true);   
    }

}