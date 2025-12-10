using Microsoft.AspNetCore.Http;
namespace ECommerce.Core.Interfaces.Services;

public interface IImageManagementService
{
    Task<List<string>> AddImageAsync(IFormFileCollection files, string src);
    Task DeleteImageFile(string src);
    Task DeleteImagesFolder(string folderName);
}