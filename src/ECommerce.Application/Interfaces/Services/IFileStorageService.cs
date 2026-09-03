namespace ECommerce.Application.Interfaces.Services;

public interface IFileStorageService
{
    Task<string> SaveAsync(
        Stream content,
        string originalFileName,
        string folder);
    Task<IReadOnlyList<string>> SaveManyAsync(
        IEnumerable<(Stream Content, string FileName)> files,
        string folder);
    Task<bool> DeleteAsync(
        string relativePath);
    Task<bool> DeleteFolderAsync(
        string folder);
}
