using ECommerce.Application.DTO.ProductImages;
using ECommerce.Domain.Entities.Product;
using ECommerce.Domain.Interfaces.Repositories;

namespace ECommerce.Application.Services;

public class ProductImageService : IProductImageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<ProductImageService> _logger;  
    private readonly FileValidationSettings _settings;
    private readonly IValidator<UploadProductImageDTO> _uploadProductImageDtoValidator;

    public ProductImageService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IFileStorageService fileStorageService,
        ILogger<ProductImageService> logger, 
        IOptions<FileValidationSettings> settings,
        IValidator<UploadProductImageDTO> uploadProductImageDtoValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fileStorageService = fileStorageService;
        _logger = logger;
        _settings = settings.Value;
        _uploadProductImageDtoValidator = uploadProductImageDtoValidator;
    }

    public async Task<ProductImageDTO> UploadImageAsync(
        UploadProductImageDTO dto,
        CancellationToken ct = default)
    {
        if (!await _unitOfWork.ProductRepository.ExistsAsync(p => p.Id == dto.ProductId, ct))
            throw new NotFoundException($"Product with ID {dto.ProductId} not found");

        var count = await _unitOfWork.ProductImageRepository.CountProductImagesAsync(dto.ProductId, ct);
        if (count >= _settings.ProductImage.MaxTotalPhotos)
        {
            throw new BadRequestException(
                $"Cannot upload photo. " +
                $"Product already has {count} photos. " +
                $"Maximum allowed is {_settings.ProductImage.MaxTotalPhotos}.");
        }

        var validationResult = await _uploadProductImageDtoValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        if (dto.IsMain)
            await ResetExistingMainImageAsync(dto.ProductId, ct);

        var folderPath = $"products/{dto.ProductId}";
        await using var stream = dto.File.OpenReadStream();

        try
        {
            var filePath = await _fileStorageService.SaveAsync(
                stream, dto.File.FileName, folderPath, ct);
            
            var image = new ProductImage
            {
                ImageUrl = filePath,
                ProductId = dto.ProductId,
                IsMain = dto.IsMain,
                AltText = dto.AltText ?? $"Product {dto.ProductId} image",
                UploadedAt = DateTime.Now
            };
            
            await _unitOfWork.ProductImageRepository.AddAsync(image, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            
            _logger.LogInformation(
                "Photo uploaded for Product {ProductId}: {FilePath} (Main:{IsMain})",
                dto.ProductId, filePath, dto.IsMain);

            return _mapper.Map<ProductImageDTO>(image);
        }
        finally
        {
            await stream.DisposeAsync();
        }
    }


    public async Task<IReadOnlyList<ProductImageDTO>> GetProductImagesAsync(
        Guid productId,
        CancellationToken ct = default)
    {
        var images = await _unitOfWork.ProductImageRepository
            .GetImagesByProductIdAsync(productId, ct);
        return _mapper.Map<IReadOnlyList<ProductImageDTO>>(images);
    }
    

    public async Task SetMainImageAsync(
        Guid productId, 
        Guid imageId, 
        CancellationToken ct = default)
    {
        var photo = await _unitOfWork.ProductImageRepository.GetByIdAsync(imageId, ct);
        
        if (photo == null || photo.ProductId != productId)
            throw new NotFoundException($"Photo {imageId} not found for product {productId}");

        if (photo.IsMain) return;
        
        var existingMain = await _unitOfWork.ProductImageRepository
            .GetProductMainImageAsync(productId, ct);
            
        if (existingMain != null && existingMain.Id != imageId)
        {
            existingMain.IsMain = false;
            await _unitOfWork.ProductImageRepository.UpdateAsync(existingMain, ct);
        }
        
        photo.IsMain = true;
        await _unitOfWork.ProductImageRepository.UpdateAsync(photo, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Photo {ImageId} set as main for Product {ProductId}", 
            imageId, productId);
    }

    public async Task DeleteProductImageAsync(
        Guid productId, 
        Guid imageId, 
        CancellationToken ct = default)
    {
        var image = await _unitOfWork.ProductImageRepository.GetByIdAsync(imageId, ct);
        if (image == null || image.ProductId != productId)
            throw new NotFoundException($"Photo {imageId} not found for product {productId}");
        
        if (image.IsMain)
        {
            var otherPhotos = await _unitOfWork.ProductImageRepository
                .GetImagesByProductIdAsync(productId, ct);
                
            if (otherPhotos.Count() > 1)
            {
                var alternative = otherPhotos.First(p => !p.IsMain);
                throw new BadRequestException(
                    $"Cannot delete main photo. Set photo {alternative.Id} as main first.");
            }
        }
        
        await _fileStorageService.DeleteAsync(image.ImageUrl, ct);
        await _unitOfWork.ProductImageRepository.DeleteAsync(image, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Product photo {ImageId} deleted from product {ProductId}", imageId, productId);
    }
    

    public async Task DeleteAllProductImagesAsync(
        Guid productId, 
        CancellationToken ct = default)
    {
        if (!await _unitOfWork.ProductRepository.ExistsAsync(p => p.Id == productId, ct))
            throw new NotFoundException($"Product with ID {productId} not found");

        var images = await _unitOfWork.ProductImageRepository
            .GetImagesByProductIdAsync(productId, ct);
        
        if (!images.Any()) return;

        foreach (var image in images)
            await _fileStorageService.DeleteAsync(image.ImageUrl, ct);

        var folderPath = $"products/{productId}";
        await _fileStorageService.DeleteFolderAsync(folderPath, ct);

        await _unitOfWork.ProductImageRepository.DeleteRangeAsync(images, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation(
            "All {Count} images deleted for Product {ProductId}",
            images.Count(), productId);
    }
    
    public async Task<ProductImageDTO?> GetImageByIdAsync(
        Guid imageId, 
        CancellationToken ct = default)
    {
        var image = await _unitOfWork.ProductImageRepository.GetByIdAsync(imageId, ct);
        if (image == null) return null;
        return _mapper.Map<ProductImageDTO>(image);
    }
    
    public async Task<ProductImageDTO?> GetProductMainImageAsync(
        Guid productId,
        CancellationToken ct = default)
    {
        var image = await _unitOfWork.ProductImageRepository
            .GetProductMainImageAsync(productId, ct);
        return image == null ? null : _mapper.Map<ProductImageDTO>(image);
    }
    

    private async Task ResetExistingMainImageAsync(
        Guid productId, 
        CancellationToken ct)
    {
        var existingMain = await _unitOfWork.ProductImageRepository
            .GetProductMainImageAsync(productId, ct);

        if (existingMain != null)
        {
            existingMain.IsMain = false;
            existingMain.UploadedAt = DateTime.UtcNow; 
            await _unitOfWork.ProductImageRepository.UpdateAsync(existingMain, ct);
        }
    }
}