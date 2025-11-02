using E_Commerece.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

namespace E_Commerece.Infrastructure.Repositories.Service;

public class ImageManagementService : IImageManagementService
{
    private readonly IFileProvider _fileProvider;

    public ImageManagementService(IFileProvider fileProvider)
    {
        _fileProvider = fileProvider;
    }
    public async Task<List<string>> AddImageAsync(IFormFileCollection files, string src)
    {
        List<string> SaveImageSrc =  new List<string>();
        string ImageDir = Path.Combine("wwwroot", "Images", src);

        if (!Directory.Exists(ImageDir))
        {
            Directory.CreateDirectory(ImageDir);
        }

        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                string ImageName = file.FileName;
                string ImageSrc = $"/Images/{src}/{ImageName}";
                string root = Path.Combine(ImageDir, ImageName);

                using (FileStream fs = new FileStream(root, FileMode.Create))
                {
                    await file.CopyToAsync(fs);
                }
                SaveImageSrc.Add(ImageSrc);
            }
        }
        
        return SaveImageSrc;
    }

    public  void DeleteImage(string src)
    {
        var info = _fileProvider.GetFileInfo(src);
        string root = info.PhysicalPath;
        File.Delete(root);
    }
}