namespace ECommerce.Application.Interfaces.Services;

public interface IFileStorageService
{
    Task<string> SaveAsync(
        Stream content,
        string originalFileName,
        string folder,
        CancellationToken ct = default);
    Task<IReadOnlyList<string>> SaveManyAsync(
        IEnumerable<(Stream Content, string FileName)> files,
        string folder,
        CancellationToken ct = default);
    Task<bool> DeleteAsync(
        string relativePath,
        CancellationToken ct = default);
    Task<bool> DeleteFolderAsync(
        string folder,
        CancellationToken ct = default);
}
