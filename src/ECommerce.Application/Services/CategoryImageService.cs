using ECommerce.Application.DTO.CategoryImages;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities.Categories;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Application.Specifications.Categories;

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
        Guid Id,
        UploadCategoryImageDTO dto,
        CancellationToken ct = default)
    {
        var categorySpec = new CategorySpecification(Id);

        var exist = await _unitOfWork.GetRepository<Category, Guid>().ExistsAsync(categorySpec);
        if (!exist)
            throw new NotFoundException($"Category {Id} not found");
        
        var categoryImageSpec = new CategoryImageSpecification(Id, dto.SubType);
        var existingPhoto = await _unitOfWork.GetRepository<CategoryImage, Guid>()
            .ExistsAsync(categoryImageSpec);

        if (existingPhoto)
        {
            throw new Exception($"{dto.SubType} already exists for Category {Id} " +
                                $"If you want to replace it, please delete the existing one first.");
            
        }

        var validationResult = await _uploadCategoryDtoValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var folderPath = $"categories/{Id}/{dto.SubType.ToString().ToLowerInvariant()}";
        await using var stream = dto.File.OpenReadStream();

        try
        {
            var filePath = await _imageService.SaveAsync(
                stream, dto.File.FileName, folderPath, ct);

            var image = new CategoryImage
            {
                ImageUrl = filePath,
                CategoryId = Id,
                SubType = dto.SubType,
                AltText = dto.AltText ?? $"{dto.SubType} for category {Id}",
                UploadedAt = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<CategoryImage, Guid>().AddAsync(image, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("{SubType} uploaded for Category {CategoryId}",
                dto.SubType, Id);

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
        var spec = new CategoryImageSpecification(categoryId);
        var image = await _unitOfWork.GetRepository<CategoryImage, Guid>()
            .GetAllAsync(spec);
        return _mapper.Map<IReadOnlyList<CategoryImageDTO>>(image);
    }
    
    public async Task<CategoryImageDTO?> GetCategoryImageBySubTypeAsync(
        Guid categoryId,
        ImageSubType subType,
        CancellationToken ct = default)
    {
        var spec = new CategoryImageSpecification(categoryId, subType);
        var image = await _unitOfWork.GetRepository<CategoryImage, Guid>()
            .GetFirstOrDefaultAsync(spec);
        return image == null ? null : _mapper.Map<CategoryImageDTO>(image);
    }
    
    public async Task DeleteCategoryImageAsync(
        Guid categoryId,
        Guid imageId,
        CancellationToken ct = default)
    {
        var spec = new CategoryImageSpecification(categoryId, imageId);
        var exist = await _unitOfWork.GetRepository<CategoryImage, Guid>().ExistsAsync(spec);
        if (!exist)
            throw new NotFoundException($"Image {imageId} not found for category {categoryId}");

        var image = await _unitOfWork.GetRepository<CategoryImage, Guid>().GetByIdAsync(imageId, ct) 
        ?? throw new NotFoundException($"Image {imageId} not found for category {categoryId}");

        
        await _imageService.DeleteAsync(image.ImageUrl, ct);
        _unitOfWork.GetRepository<CategoryImage, Guid>().Delete(image, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Category Image {ImageId} deleted from category {CategoryId}", 
            imageId, categoryId);
    }
    
    public async Task DeleteAllCategoryImagesAsync(
        Guid categoryId,
        CancellationToken ct = default)
    {
        var spec = new CategoryImageSpecification(categoryId);
        var images = await _unitOfWork.GetRepository<CategoryImage, Guid>()
            .GetAllAsync(spec);
        
        if (!images.Any()) return;

        foreach (var image in images)
            await _imageService.DeleteAsync(image.ImageUrl, ct);

        var folderPath = $"categories/{categoryId}";
        await _imageService.DeleteFolderAsync(folderPath, ct);

        _unitOfWork.GetRepository<CategoryImage, Guid>().DeleteRange(images, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("All {Count} photos deleted for Category {CategoryId}", 
            images.Count(), categoryId);
    }
    
    public async Task<CategoryImageDTO?> GetImageByIdAsync(
        Guid categoryId,
        Guid imageId,
        CancellationToken ct = default)
    {
        var spec = new CategoryImageSpecification(categoryId, imageId);
        var exist = await _unitOfWork.GetRepository<CategoryImage, Guid>().ExistsAsync(spec);
        if (!exist)
            throw new NotFoundException($"Image {imageId} not found for category {categoryId}");
        var image = await _unitOfWork.GetRepository<CategoryImage, Guid>().GetFirstOrDefaultAsync(spec);
        return _mapper.Map<CategoryImageDTO>(image);
    }


}