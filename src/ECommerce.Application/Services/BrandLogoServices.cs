using ECommerce.Application.DTO.BrandLogos;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities.Brands;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Application.Specifications.Brands;


namespace ECommerce.Application.Services;

public class BrandLogoService : IBrandLogoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFileStorageService _logoService;
    private readonly ILogger<BrandLogoService> _logger;
    private readonly FileValidationSettings _settings;
    private readonly IValidator<UploadBrandLogoDTO> _uploadBrandDtoValidator;

    public BrandLogoService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IFileStorageService logoService,
        ILogger<BrandLogoService> logger,
        IOptions<FileValidationSettings> settings,
        IValidator<UploadBrandLogoDTO> uploadBrandDtoValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logoService = logoService;
        _logger = logger;
        _settings = settings.Value;
        _uploadBrandDtoValidator = uploadBrandDtoValidator;
    }
    
    public async Task<BrandLogoDTO> UploadlogoAsync(
        Guid brandId,
        UploadBrandLogoDTO dto)
    {
        var brandSpec = new BrandSpecification(brandId);

        var exist = await _unitOfWork.GetRepository<Brand, Guid>().ExistsAsync(brandSpec);
        if (!exist)
            throw new NotFoundException($"Brand {brandId} not found");
        
        var BrandLogoSpec = new BrandLogoSpecification(brandId, dto.SubType);
        var existingPhoto = await _unitOfWork.GetRepository<BrandLogo, Guid>()
            .ExistsAsync(BrandLogoSpec);

        if (existingPhoto)
        {
            throw new Exception($"{dto.SubType} already exists for Brand {brandId} " +
                                $"If you want to replace it, please delete the existing one first.");
            
        }

        var validationResult = await _uploadBrandDtoValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var folderPath = $"categories/{brandId}/{dto.SubType.ToString().ToLowerInvariant()}";
        await using var stream = dto.File.OpenReadStream();

        try
        {
            var filePath = await _logoService.SaveAsync(
                stream, dto.File.FileName, folderPath);

            var logo = new BrandLogo
            {
                ImageUrl = filePath,
                BrandId = brandId,
                SubType = dto.SubType,
                AltText = dto.AltText ?? $"{dto.SubType} for Brand {brandId}",
                UploadedAt = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<BrandLogo, Guid>().AddAsync(logo);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("{SubType} uploaded for Brand {BrandId}",
                dto.SubType, brandId);

            return _mapper.Map<BrandLogoDTO>(logo);
        }

        finally
        {
            await stream.DisposeAsync();
        }
    }
    
    public async Task<IReadOnlyList<BrandLogoDTO>> GetBrandLogosAsync(
        Guid BrandId)
    {
        var spec = new BrandLogoSpecification(BrandId);
        var logo = await _unitOfWork.GetRepository<BrandLogo, Guid>()
            .GetAllAsync(spec);
        return _mapper.Map<IReadOnlyList<BrandLogoDTO>>(logo);
    }
    
    public async Task<BrandLogoDTO?> GetBrandLogoBySubTypeAsync(
        Guid BrandId,
        ImageSubType subType)
    {
        var spec = new BrandLogoSpecification(BrandId, subType);
        var logo = await _unitOfWork.GetRepository<BrandLogo, Guid>()
            .GetFirstOrDefaultAsync(spec);
        return logo == null ? null : _mapper.Map<BrandLogoDTO>(logo);
    }
    
    public async Task DeleteBrandLogoAsync(
        Guid brandId,
        Guid logoId)
    {
        var spec = new BrandLogoSpecification(brandId, logoId);
        var exist = await _unitOfWork.GetRepository<BrandLogo, Guid>().ExistsAsync(spec);
        if (!exist)
            throw new NotFoundException($"Logo {logoId} not found for Brand {brandId}");

        var logo = await _unitOfWork.GetRepository<BrandLogo, Guid>().GetByIdAsync(logoId) 
        ?? throw new NotFoundException($"Logo {logoId} not found for Brand {brandId}");

        
        await _logoService.DeleteAsync(logo.ImageUrl);
        _unitOfWork.GetRepository<BrandLogo, Guid>().Delete(logo);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Brand Logo {logoId} deleted from Brand {brandId}", 
            logoId, brandId);
    }
    
    public async Task DeleteAllBrandLogosAsync(
        Guid BrandId)
    {
        var spec = new BrandLogoSpecification(BrandId);
        var logos = await _unitOfWork.GetRepository<BrandLogo, Guid>()
            .GetAllAsync(spec);
        
        if (!logos.Any()) return;

        foreach (var logo in logos)
            await _logoService.DeleteAsync(logo.ImageUrl);

        var folderPath = $"categories/{BrandId}";
        await _logoService.DeleteFolderAsync(folderPath);

        _unitOfWork.GetRepository<BrandLogo, Guid>().DeleteRange(logos);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("All {Count} photos deleted for Brand {BrandId}", 
            logos.Count(), BrandId);
    }
    
    public async Task<BrandLogoDTO?> GetLogoByIdAsync(
        Guid brandId,
        Guid logoId)
    {
        var spec = new BrandLogoSpecification(brandId, logoId);
        var exists = await _unitOfWork.GetRepository<BrandLogo, Guid>().ExistsAsync(spec);
        
        if (!exists) 
            throw new NotFoundException($"Logo {logoId} not found for Brand {brandId}");   

        var logo = await _unitOfWork.GetRepository<BrandLogo, Guid>().GetFirstOrDefaultAsync(spec);
        return _mapper.Map<BrandLogoDTO>(logo);
    }


}