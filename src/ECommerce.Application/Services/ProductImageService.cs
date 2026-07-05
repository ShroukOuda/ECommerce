using ECommerce.Application.DTO.ProductImages;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities.Products;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Application.Specifications.Products;

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
        var productSpec = new ProductSpecification(dto.ProductId);
        bool exist = await _unitOfWork.GetRepository<Product, Guid>().ExistsAsync(productSpec);
        if (!exist)
            throw new NotFoundException($"Product with ID {dto.ProductId} not found");
        
        var productImageSpec = new ProductImageSpecification(dto.ProductId);
        var count = await _unitOfWork.GetRepository<ProductImage, Guid>().CountAsync(productImageSpec);

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
            
            await _unitOfWork.GetRepository<ProductImage, Guid>().AddAsync(image, ct);
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
        var spec = new ProductImageSpecification(productId);
        var images = await _unitOfWork.GetRepository<ProductImage, Guid>()
            .GetAllAsync(spec);
        return _mapper.Map<IReadOnlyList<ProductImageDTO>>(images);
    }
    

    public async Task SetMainImageAsync(
        Guid productId, 
        Guid imageId, 
        CancellationToken ct = default)
    {
        var photo = await _unitOfWork.GetRepository<ProductImage, Guid>().GetByIdAsync(imageId, ct);
        
        if (photo == null || photo.ProductId != productId)
            throw new NotFoundException($"Photo {imageId} not found for product {productId}");

        if (photo.IsMain) return;
        
        var mainImageSpec = new ProductImageSpecification(productId, true);
        var existingMain = await _unitOfWork.GetRepository<ProductImage, Guid>()
            .GetFirstOrDefaultAsync(mainImageSpec);
            
        if (existingMain != null && existingMain.Id != imageId)
        {
            existingMain.IsMain = false;
            _unitOfWork.GetRepository<ProductImage, Guid>().Update(existingMain, ct);
        }
        
        photo.IsMain = true;
        _unitOfWork.GetRepository<ProductImage, Guid>().Update(photo, ct);
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
        var image = await _unitOfWork.GetRepository<ProductImage, Guid>().GetByIdAsync(imageId, ct);
        if (image == null || image.ProductId != productId)
            throw new NotFoundException($"Photo {imageId} not found for product {productId}");
        
        if (image.IsMain)
        {
            var spec = new ProductImageSpecification(productId);
            var otherPhotosCount = await _unitOfWork.GetRepository<ProductImage, Guid>()
                .CountAsync(spec);
                
            if (otherPhotosCount > 1)
            {
                throw new BadRequestException(
                    $"Cannot delete main photo. Set any photo as main first.");
            }
        }
        
        await _fileStorageService.DeleteAsync(image.ImageUrl, ct);
        _unitOfWork.GetRepository<ProductImage, Guid>().Delete(image, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Product photo {ImageId} deleted from product {ProductId}", imageId, productId);
    }
    

    public async Task DeleteAllProductImagesAsync(
        Guid productId, 
        CancellationToken ct = default)
    {
        var productSpec = new ProductSpecification(productId);
        var exist = await _unitOfWork.GetRepository<Product, Guid>().ExistsAsync(productSpec);
        if (!exist)
            throw new NotFoundException($"Product with ID {productId} not found");

        var productImageSpec = new ProductImageSpecification(productId);
        var images = await _unitOfWork.GetRepository<ProductImage, Guid>()
            .GetAllAsync(productImageSpec);
        
        if (!images.Any()) return;

        foreach (var image in images)
            await _fileStorageService.DeleteAsync(image.ImageUrl, ct);

        var folderPath = $"products/{productId}";
        await _fileStorageService.DeleteFolderAsync(folderPath, ct);

        _unitOfWork.GetRepository<ProductImage, Guid>().DeleteRange(images, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation(
            "All {Count} images deleted for Product {ProductId}",
            images.Count(), productId);
    }
    
    public async Task<ProductImageDTO?> GetImageByIdAsync(
        Guid imageId, 
        CancellationToken ct = default)
    {
        var image = await _unitOfWork.GetRepository<ProductImage, Guid>().GetByIdAsync(imageId, ct);
        if (image == null) return null;
        return _mapper.Map<ProductImageDTO>(image);
    }
    
    public async Task<ProductImageDTO?> GetProductMainImageAsync(
        Guid productId,
        CancellationToken ct = default)
    {
        var spec = new ProductImageSpecification(productId, true);
        var image = await _unitOfWork.GetRepository<ProductImage, Guid>()
            .GetFirstOrDefaultAsync(spec);
        return image == null ? null : _mapper.Map<ProductImageDTO>(image);
    }
    

    private async Task ResetExistingMainImageAsync(
        Guid productId, 
        CancellationToken ct)
    {
        var spec = new ProductImageSpecification(productId, true);
        var existingMain = await _unitOfWork.GetRepository<ProductImage, Guid>()
            .GetFirstOrDefaultAsync(spec);

        if (existingMain != null)
        {
            existingMain.IsMain = false;
            existingMain.UploadedAt = DateTime.UtcNow; 
            _unitOfWork.GetRepository<ProductImage, Guid>().Update(existingMain, ct);
        }
    }
}