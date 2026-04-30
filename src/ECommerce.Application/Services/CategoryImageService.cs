using ECommerce.Application.DTO.CategoryImages;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities.Category;
using ECommerce.Domain.Interfaces.Repositories;

namespace ECommerce.Application.Services;

public class CategoryImageService : ICategoryImageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFileStorageService _imageService;
    private readonly ILogger<CategoryImageService> _logger;
    private readonly FileValidationSettings _settings;
    private readonly IValidator<UploadCategoryImageDTO> _uploadCategoryDtoValidator;

    public CategoryImageService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IFileStorageService imageService,
        ILogger<CategoryImageService> logger,
        IOptions<FileValidationSettings> settings,
        IValidator<UploadCategoryImageDTO> uploadCategoryDtoValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _imageService = imageService;
        _logger = logger;
        _settings = settings.Value;
        _uploadCategoryDtoValidator = uploadCategoryDtoValidator;
    }
    
    public async Task<CategoryImageDTO> UploadImageAsync(
        UploadCategoryImageDTO dto,
        CancellationToken ct = default)
    {
    
        if (!await _unitOfWork.CategoryRepository.ExistsAsync(c => c.Id == dto.CategoryId, ct))
            throw new NotFoundException($"Category {dto.CategoryId} not found");
        
        var existingPhoto = await _unitOfWork.CategoryImageRepository
            .GetCategoryImageBySubTypeAsync(dto.CategoryId, dto.SubType, ct);

        if (existingPhoto != null)
        {
            throw new Exception($"{dto.SubType} already exists for Category {dto.CategoryId} " +
                                $"If you want to replace it, please delete the existing one first.");
            
        }

        var validationResult = await _uploadCategoryDtoValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var folderPath = $"categories/{dto.CategoryId}/{dto.SubType.ToString().ToLowerInvariant()}";
        await using var stream = dto.File.OpenReadStream();

        try
        {
            var filePath = await _imageService.SaveAsync(
                stream, dto.File.FileName, folderPath, ct);

            var image = new CategoryImage
            {
                ImageUrl = filePath,
                CategoryId = dto.CategoryId,
                SubType = dto.SubType,
                AltText = dto.AltText ?? $"{dto.SubType} for category {dto.CategoryId}",
                UploadedAt = DateTime.UtcNow
            };

            await _unitOfWork.CategoryImageRepository.AddAsync(image, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("{SubType} uploaded for Category {CategoryId}",
                dto.SubType, dto.CategoryId);

            return _mapper.Map<CategoryImageDTO>(image);
        }

        finally
        {
            await stream.DisposeAsync();
        }
    }
    
    public async Task<IReadOnlyList<CategoryImageDTO>> GetCategoryImagesAsync(
        Guid categoryId,
        CancellationToken ct = default)
    {
        var image = await _unitOfWork.CategoryImageRepository
            .GetImagesByCategoryIdAsync(categoryId, ct);
        return _mapper.Map<IReadOnlyList<CategoryImageDTO>>(image);
    }
    
    public async Task<CategoryImageDTO?> GetCategoryImageBySubTypeAsync(
        Guid categoryId,
        ImageSubType subType,
        CancellationToken ct = default)
    {
        var image = await _unitOfWork.CategoryImageRepository
            .GetCategoryImageBySubTypeAsync(categoryId, subType, ct);
        return image == null ? null : _mapper.Map<CategoryImageDTO>(image);
    }
    
    public async Task DeleteCategoryImageAsync(
        Guid categoryId,
        Guid imageId,
        CancellationToken ct = default)
    {
        var image = await _unitOfWork.CategoryImageRepository.GetByIdAsync(imageId, ct);
        if (image == null || image.CategoryId != categoryId)
            throw new NotFoundException($"Image {imageId} not found for category {categoryId}");
        
        await _imageService.DeleteAsync(image.ImageUrl, ct);
        await _unitOfWork.CategoryImageRepository.DeleteAsync(image, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Category Image {ImageId} deleted from category {CategoryId}", 
            imageId, categoryId);
    }
    
    public async Task DeleteAllCategoryImagesAsync(
        Guid categoryId,
        CancellationToken ct = default)
    {
        var images = await _unitOfWork.CategoryImageRepository
            .GetImagesByCategoryIdAsync(categoryId, ct);
        
        if (!images.Any()) return;

        foreach (var image in images)
            await _imageService.DeleteAsync(image.ImageUrl, ct);

        var folderPath = $"categories/{categoryId}";
        await _imageService.DeleteFolderAsync(folderPath, ct);

        await _unitOfWork.CategoryImageRepository.DeleteRangeAsync(images, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("All {Count} photos deleted for Category {CategoryId}", 
            images.Count(), categoryId);
    }
    
    public async Task<CategoryImageDTO?> GetImageByIdAsync(
        Guid imageId,
        CancellationToken ct = default)
    {
        var image = await _unitOfWork.CategoryImageRepository.GetByIdAsync(imageId, ct);
        if (image == null) return null;
        return _mapper.Map<CategoryImageDTO>(image);
    }


}