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
        UploadBrandLogoDTO dto,
        CancellationToken ct = default)
    {
        var brandSpec = new BrandSpecification(dto.BrandId);

        var exist = await _unitOfWork.GetRepository<Brand, Guid>().ExistsAsync(brandSpec);
        if (!exist)
            throw new NotFoundException($"Brand {dto.BrandId} not found");
        
        var BrandLogoSpec = new BrandLogoSpecification(dto.BrandId, dto.SubType);
        var existingPhoto = await _unitOfWork.GetRepository<BrandLogo, Guid>()
            .ExistsAsync(BrandLogoSpec);

        if (existingPhoto)
        {
            throw new Exception($"{dto.SubType} already exists for Brand {dto.BrandId} " +
                                $"If you want to replace it, please delete the existing one first.");
            
        }

        var validationResult = await _uploadBrandDtoValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var folderPath = $"categories/{dto.BrandId}/{dto.SubType.ToString().ToLowerInvariant()}";
        await using var stream = dto.File.OpenReadStream();

        try
        {
            var filePath = await _logoService.SaveAsync(
                stream, dto.File.FileName, folderPath, ct);

            var logo = new BrandLogo
            {
                ImageUrl = filePath,
                BrandId = dto.BrandId,
                SubType = dto.SubType,
                AltText = dto.AltText ?? $"{dto.SubType} for Brand {dto.BrandId}",
                UploadedAt = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<BrandLogo, Guid>().AddAsync(logo, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("{SubType} uploaded for Brand {BrandId}",
                dto.SubType, dto.BrandId);

            return _mapper.Map<BrandLogoDTO>(logo);
        }

        finally
        {
            await stream.DisposeAsync();
        }
    }
    
    public async Task<IReadOnlyList<BrandLogoDTO>> GetBrandLogosAsync(
        Guid BrandId,
        CancellationToken ct = default)
    {
        var spec = new BrandLogoSpecification(BrandId);
        var logo = await _unitOfWork.GetRepository<BrandLogo, Guid>()
            .GetAllAsync(spec);
        return _mapper.Map<IReadOnlyList<BrandLogoDTO>>(logo);
    }
    
    public async Task<BrandLogoDTO?> GetBrandLogoBySubTypeAsync(
        Guid BrandId,
        ImageSubType subType,
        CancellationToken ct = default)
    {
        var spec = new BrandLogoSpecification(BrandId, subType);
        var logo = await _unitOfWork.GetRepository<BrandLogo, Guid>()
            .GetFirstOrDefaultAsync(spec);
        return logo == null ? null : _mapper.Map<BrandLogoDTO>(logo);
    }
    
    public async Task DeleteBrandLogoAsync(
        Guid brandId,
        Guid logoId,
        CancellationToken ct = default)
    {
        var spec = new BrandLogoSpecification(brandId, logoId);
        var exist = await _unitOfWork.GetRepository<BrandLogo, Guid>().ExistsAsync(spec);
        if (!exist)
            throw new NotFoundException($"Logo {logoId} not found for Brand {brandId}");

        var logo = await _unitOfWork.GetRepository<BrandLogo, Guid>().GetByIdAsync(logoId, ct) 
        ?? throw new NotFoundException($"Logo {logoId} not found for Brand {brandId}");

        
        await _logoService.DeleteAsync(logo.ImageUrl, ct);
        _unitOfWork.GetRepository<BrandLogo, Guid>().Delete(logo, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Brand Logo {logoId} deleted from Brand {brandId}", 
            logoId, brandId);
    }
    
    public async Task DeleteAllBrandLogosAsync(
        Guid BrandId,
        CancellationToken ct = default)
    {
        var spec = new BrandLogoSpecification(BrandId);
        var logos = await _unitOfWork.GetRepository<BrandLogo, Guid>()
            .GetAllAsync(spec);
        
        if (!logos.Any()) return;

        foreach (var logo in logos)
            await _logoService.DeleteAsync(logo.ImageUrl, ct);

        var folderPath = $"categories/{BrandId}";
        await _logoService.DeleteFolderAsync(folderPath, ct);

        _unitOfWork.GetRepository<BrandLogo, Guid>().DeleteRange(logos, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("All {Count} photos deleted for Brand {BrandId}", 
            logos.Count(), BrandId);
    }
    
    public async Task<BrandLogoDTO?> GetLogoByIdAsync(
        Guid logoId,
        CancellationToken ct = default)
    {
        var logo = await _unitOfWork.GetRepository<BrandLogo, Guid>().GetByIdAsync(logoId, ct);
        if (logo == null) return null;
        return _mapper.Map<BrandLogoDTO>(logo);
    }


}