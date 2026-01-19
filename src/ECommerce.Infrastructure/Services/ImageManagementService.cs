using Microsoft.Extensions.FileProviders;
using ECommerce.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ECommerce.Infrastructure.Services;
public class ImageManagementService : IImageManagementService
{
   
    private readonly IFileProvider _fileProvider;
    private readonly FileStorageSettings _settings;
    private readonly ILogger<ImageManagementService> _logger;
   

    public ImageManagementService(
        IFileProvider fileProvider,
        IOptions<FileStorageSettings> settings,
        ILogger<ImageManagementService> logger)
    {
        _fileProvider = fileProvider;
        _settings = settings.Value;
        _logger = logger;
    }
    
    public async Task<string> SaveAsync(
        Stream content,
        string originalFileName,
        string folder,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(originalFileName);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(folder);
        
        var fileName = GenerateFileName(originalFileName);
        var relativePath = Path.Combine(_settings.BasePath, folder, fileName);

        var fileInfo = _fileProvider.GetFileInfo(relativePath);

        if (fileInfo.PhysicalPath is null)
            throw new InvalidOperationException("Physical storage is not available.");

        var directory = Path.GetDirectoryName(fileInfo.PhysicalPath)!;
        Directory.CreateDirectory(directory);

        await using var stream = new FileStream(
            fileInfo.PhysicalPath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 81920,
            useAsync: true);

        await content.CopyToAsync(stream, ct);
        
        return relativePath.Replace("\\", "/");
    }
    
    public async Task<IReadOnlyList<string>> SaveManyAsync(
        IEnumerable<(Stream Content, string FileName)> files,
        string folder,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(files);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(folder);
        
        var savedPaths = new List<string>();

        foreach (var (content, fileName) in files)
        {
            ct.ThrowIfCancellationRequested();

            var path = await SaveAsync(
                content,
                fileName,
                folder,
                ct);

            savedPaths.Add(path);
        }

        return savedPaths;
    }
    
    public Task<bool> DeleteAsync(
        string relativePath,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return Task.FromResult(false);

        var fileInfo = _fileProvider.GetFileInfo(relativePath);

        if (fileInfo.PhysicalPath is null || !File.Exists(fileInfo.PhysicalPath))
            return Task.FromResult(false);

        try
        {
            File.Delete(fileInfo.PhysicalPath);
            return Task.FromResult(true);
        }
        catch (IOException)
        {
            return Task.FromResult(false);
        }
        catch (UnauthorizedAccessException)
        {
            return Task.FromResult(false);
        }
       
    }

    public Task<bool> DeleteFolderAsync(
        string folder,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(folder))
            return Task.FromResult(false);

        var relativePath = Path.Combine(_settings.BasePath, folder);
      
        var parentPath = Path.GetDirectoryName(relativePath) ?? _settings.BasePath;
        var folderName = Path.GetFileName(relativePath);
    
        var parentContents = _fileProvider.GetDirectoryContents(parentPath);
    
        if (!parentContents.Exists)
        {
            Console.WriteLine($"Parent directory doesn't exist: '{parentPath}'");
            return Task.FromResult(false);
        }
    
      
        var targetFolder = parentContents
            .FirstOrDefault(f => f.IsDirectory && 
                                 f.Name.Equals(folderName, StringComparison.OrdinalIgnoreCase));
    
        if (targetFolder is null)
        {
            Console.WriteLine($"Folder '{folderName}' not found in '{parentPath}'");
            return Task.FromResult(false);
        }
    
        if (targetFolder.PhysicalPath is null)
        {
            Console.WriteLine($"Physical path is null for folder '{folderName}'");
            return Task.FromResult(false);
        }
    
        var directoryPath = targetFolder.PhysicalPath;
        Console.WriteLine($"Found folder at: '{directoryPath}'");
    
       
        if (!Directory.Exists(directoryPath))
        {
            return Task.FromResult(false);
        }
    
        try
        {
            Directory.Delete(directoryPath, recursive: true);
            return Task.FromResult(true);
        }
        catch (IOException ex)
        {
            return Task.FromResult(false);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Task.FromResult(false);
        }
    }

    private string GenerateFileName(string originalFileName)
    {
        var extension = Path.GetExtension(originalFileName).ToLowerInvariant();

        return _settings.NamingStrategy switch
        {
            FileNamingStrategy.Guid =>
                $"{Guid.NewGuid()}{extension}",

            FileNamingStrategy.Timestamp =>
                $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}{extension}",

            FileNamingStrategy.Original =>
                SanitizeFileName(originalFileName),

            _ =>
                $"{Guid.NewGuid()}{extension}"
        };
    }

    private static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var name = string.Join("_", fileName.Split(invalidChars));

        var extension = Path.GetExtension(name);
        var baseName = Path.GetFileNameWithoutExtension(name);

        return $"{baseName}_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}{extension}";
    }
    
    private Task<bool> DeletePhysicalFolder(string physicalPath)
    {
        try
        {
            if (!Directory.Exists(physicalPath))
                return Task.FromResult(false);
            
            Directory.Delete(physicalPath, recursive: true);
            _logger.LogInformation("Deleted folder: {Path}", physicalPath);
            return Task.FromResult(true);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            _logger.LogError(ex, "Failed to delete folder: {Path}", physicalPath);
            return Task.FromResult(false);
        }
    }
}