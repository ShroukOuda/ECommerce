using Microsoft.AspNetCore.Http;
namespace E_Commerece.Core.Services;

public interface IImageManagementService
{
    Task<List<string>> AddImageAsync(IFormFileCollection files, string src);
    Task DeleteImageAsync(string src);
}