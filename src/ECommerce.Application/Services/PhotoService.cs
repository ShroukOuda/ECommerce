using ECommerce.Core.Exceptions;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Services;

public class PhotoService : IPhotoService
{
     private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IImageManagementService _imageManagementService;
    private readonly ILogger<PhotoService> _logger;

    public PhotoService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IImageManagementService imageManagementService,
        ILogger<PhotoService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _imageManagementService = imageManagementService;
        _logger = logger;
    }
    
    public async Task<PhotoDTO> UploadPhotoAsync(
        UploadPhotoDTO uploadPhotoDto,
        CancellationToken ct = default)
    {
        await ValidateEntityExistsAsync(uploadPhotoDto.Type, uploadPhotoDto.EntityId);
        
        await HandleExistingPhotoAsync(uploadPhotoDto, ct);
        
        var filePath = await UploadFileAsync(uploadPhotoDto, ct);
        
        var photo = CreatePhotoEntity(uploadPhotoDto, filePath);
        
        await _unitOfWork.PhotoRepository.AddAsync(photo, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Photo uploaded for {Type} with ID {EntityId}: {FilePath}",
            uploadPhotoDto.Type,
            uploadPhotoDto.EntityId,
            filePath);

        return _mapper.Map<PhotoDTO>(photo);
    }

    public async Task<IReadOnlyList<PhotoDTO>> UploadPhotosAsync(
        UploadPhotosDTO uploadPhotosDto,
        CancellationToken ct = default)
    {
        await ValidateEntityExistsAsync(uploadPhotosDto.Type, uploadPhotosDto.EntityId);
        
        var filePaths = await UploadMultipleFilesAsync(uploadPhotosDto, ct);
        
        var existingPhotos = await GetExistingPhotosAsync(
            uploadPhotosDto.Type,
            uploadPhotosDto.EntityId,
            ct);

        var shouldSetFirstAsMain = uploadPhotosDto.MakeFirstAsMain && 
                                   !existingPhotos.Any(p => p.IsMain);
        
        var photos = CreatePhotoEntities(uploadPhotosDto, filePaths, shouldSetFirstAsMain);
        
        await _unitOfWork.PhotoRepository.AddRangeAsync(photos, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation(
            "{Count} photos uploaded for {Type} with ID {EntityId}",
            photos.Count,
            uploadPhotosDto.Type,
            uploadPhotosDto.EntityId);

        return _mapper.Map<IReadOnlyList<PhotoDTO>>(photos);
    }
    

    public async Task DeletePhotoAsync(int photoId, CancellationToken ct = default)
    {
        var photo = await _unitOfWork.PhotoRepository.GetByIdAsync(photoId, ct);
        if (photo == null)
            throw new NotFoundException($"Photo with ID {photoId} not found");
        
        await ValidateMainPhotoDeletionAsync(photo, ct);
        
        await DeletePhotoWithFileAsync(photo, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Photo {PhotoId} deleted", photoId);
    }

    public async Task DeleteEntityPhotosAsync(
        PhotoType type,
        int entityId,
        CancellationToken ct = default)
    {
        await ValidateEntityExistsAsync(type, entityId);

        var photos = await GetExistingPhotosAsync(type, entityId, ct);
        
        if (!photos.Any())
            return;
        
        foreach (var photo in photos)
        {
            await _imageManagementService.DeleteAsync(photo.Url, ct);
        }
        
        var folderPath = GetFolderPath(type, entityId, null);
        await _imageManagementService.DeleteFolderAsync(folderPath, ct);
        
        await _unitOfWork.PhotoRepository.DeleteRangeAsync(photos, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation(
            "All {Count} photos deleted for {Type} ID {EntityId}",
            photos.Count(),
            type,
            entityId);
    }

    public async Task SetMainPhotoAsync(int photoId, CancellationToken ct = default)
    {
        var photo = await _unitOfWork.PhotoRepository.GetByIdAsync(photoId, ct);
        if (photo == null)
            throw new NotFoundException($"Photo with ID {photoId} not found");

        if (photo.IsMain)
            return; 

        var entityId = int.Parse(photo.EntityId);
        
        await ResetExistingMainPhotoAsync(photo.Type, entityId, ct);
        
        photo.IsMain = true;
        await _unitOfWork.PhotoRepository.UpdateAsync(photo, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Photo {PhotoId} set as main for {Type} ID {EntityId}",
            photoId,
            photo.Type,
            entityId);
    }
    

    public async Task<IReadOnlyList<PhotoDTO>> GetEntityPhotosAsync(
        PhotoType type,
        int entityId,
        CancellationToken ct = default)
    {
        var photos = await _unitOfWork.PhotoRepository.GetPhotosByEntityAsync(
            type,
            entityId.ToString(),
            ct);

        return _mapper.Map<IReadOnlyList<PhotoDTO>>(photos);
    }
    

    private async Task ValidateEntityExistsAsync(PhotoType type, int entityId)
    {
        var exists = type switch
        {
            PhotoType.ProductImage => await _unitOfWork.ProductRepository
                .ExistsAsync(p => p.Id == entityId),
            PhotoType.CategoryMedia => await _unitOfWork.CategoryRepository
                .ExistsAsync(c => c.Id == entityId),
            _ => throw new BadRequestException($"Unsupported photo type: {type}")
        };

        if (!exists)
        {
            var entityName = type switch
            {
                PhotoType.ProductImage => "Product",
                PhotoType.CategoryMedia => "Category",
                _ => "Entity"
            };
            throw new NotFoundException($"{entityName} with ID {entityId} not found");
        }
    }

    private async Task HandleExistingPhotoAsync(
        UploadPhotoDTO uploadPhotoDto,
        CancellationToken ct)
    {
        if (uploadPhotoDto.SubType.HasValue)
        {
            var existingPhoto = await _unitOfWork.PhotoRepository.GetBySubTypeAsync(
                uploadPhotoDto.Type,
                uploadPhotoDto.EntityId.ToString(),
                uploadPhotoDto.SubType.Value,
                ct);

            if (existingPhoto != null)
            {
                await DeletePhotoWithFileAsync(existingPhoto, ct);
            }
        }
        else if (uploadPhotoDto.IsMain)
        {
            await ResetExistingMainPhotoAsync(
                uploadPhotoDto.Type,
                uploadPhotoDto.EntityId,
                ct);
        }
    }

    private async Task<string> UploadFileAsync(
        UploadPhotoDTO uploadPhotoDto,
        CancellationToken ct)
    {
        var folderPath = GetFolderPath(
            uploadPhotoDto.Type,
            uploadPhotoDto.EntityId,
            uploadPhotoDto.SubType);

        await using var stream = uploadPhotoDto.File.OpenReadStream();
        return await _imageManagementService.SaveAsync(
            stream,
            uploadPhotoDto.File.FileName,
            folderPath,
            ct);
    }

    private async Task<IReadOnlyList<string>> UploadMultipleFilesAsync(
        UploadPhotosDTO uploadPhotosDto,
        CancellationToken ct)
    {
        var folderPath = GetFolderPath(uploadPhotosDto.Type, uploadPhotosDto.EntityId, null);
        var files = new List<(Stream Content, string FileName)>();

        try
        {
            foreach (var file in uploadPhotosDto.Files)
            {
                files.Add((file.OpenReadStream(), file.FileName));
            }

            return await _imageManagementService.SaveManyAsync(files, folderPath, ct);
        }
        finally
        {
            foreach (var (stream, _) in files)
                await stream.DisposeAsync();
        }
    }

    private Photo CreatePhotoEntity(UploadPhotoDTO dto, string filePath)
    {
        var photo = new Photo
        {
            Url = filePath,
            Type = dto.Type,
            SubType = dto.SubType,
            EntityId = dto.EntityId.ToString(),
            IsMain = dto.IsMain,
            AltText = dto.AltText
        };

        SetNavigationProperty(photo, dto.Type, dto.EntityId);
        return photo;
    }

    private List<Photo> CreatePhotoEntities(
        UploadPhotosDTO dto,
        IReadOnlyList<string> filePaths,
        bool setFirstAsMain)
    {
        var photos = new List<Photo>();

        for (int i = 0; i < filePaths.Count; i++)
        {
            var photo = new Photo
            {
                Url = filePaths[i],
                Type = dto.Type,
                EntityId = dto.EntityId.ToString(),
                IsMain = setFirstAsMain && i == 0,
                AltText = dto.AltTexts?.ElementAtOrDefault(i)
            };

            SetNavigationProperty(photo, dto.Type, dto.EntityId);
            photos.Add(photo);
        }

        return photos;
    }

    private async Task ValidateMainPhotoDeletionAsync(Photo photo, CancellationToken ct)
    {
        if (!photo.IsMain)
            return;

        var otherPhotos = await GetExistingPhotosAsync(
            photo.Type,
            int.Parse(photo.EntityId),
            ct);

        if (otherPhotos.Count() > 1)
        {
            throw new BadRequestException(
                "Cannot delete main photo. Set another photo as main first.");
        }
    }
    private async Task DeletePhotoWithFileAsync(Photo photo, CancellationToken ct)
    {
        try
        {
            await _imageManagementService.DeleteAsync(photo.Url, ct);
            await _unitOfWork.PhotoRepository.DeleteAsync(photo, ct);

            _logger.LogDebug("Photo file deleted: {Url}", photo.Url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete photo file: {Url}", photo.Url);
            throw new FileOperationException("Failed to delete photo file", ex);
        }
    }

    private async Task<IEnumerable<Photo>> GetExistingPhotosAsync(
        PhotoType type,
        int entityId,
        CancellationToken ct)
    {
        return await _unitOfWork.PhotoRepository.GetPhotosByEntityAsync(
            type,
            entityId.ToString(),
            ct);
    }

    private async Task ResetExistingMainPhotoAsync(
        PhotoType type,
        int entityId,
        CancellationToken ct)
    {
        var existingMain = (await GetExistingPhotosAsync(type, entityId, ct))
            .FirstOrDefault(p => p.IsMain);

        if (existingMain != null)
        {
            existingMain.IsMain = false;
            await _unitOfWork.PhotoRepository.UpdateAsync(existingMain, ct);
        }
    }

    private string GetFolderPath(
        PhotoType type,
        int entityId,
        PhotoSubType? subType = null)
    {
        var basePath = type switch
        {
            PhotoType.ProductImage => "products",
            PhotoType.CategoryMedia => "categories",
            _ => "general"
        };

        var path = $"{basePath}/{entityId}";

        if (subType.HasValue)
        {
            var subTypeFolder = subType.Value.ToString().ToLowerInvariant();
            path = $"{path}/{subTypeFolder}";
        }

        return path;
    }

    private void SetNavigationProperty(Photo photo, PhotoType type, int entityId)
    {
        switch (type)
        {
            case PhotoType.ProductImage:
                photo.ProductId = entityId;
                break;
            case PhotoType.CategoryMedia:
                photo.CategoryId = entityId;
                break;
        }
    }
}
