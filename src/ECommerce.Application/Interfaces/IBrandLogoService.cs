using ECommerce.Application.DTO.BrandLogos;

namespace ECommerce.Application.Interfaces;

public interface IBrandLogoService
{
    Task<BrandLogoDTO> UploadlogoAsync(
        UploadBrandLogoDTO dto, 
        CancellationToken ct = default);
    
    Task<IReadOnlyList<BrandLogoDTO>> GetBrandLogosAsync(
        Guid brandId, 
        CancellationToken ct = default);
    Task<BrandLogoDTO?> GetBrandLogoBySubTypeAsync(
        Guid brandId, 
        ImageSubType subType, 
        CancellationToken ct = default);
    
    Task<BrandLogoDTO?> GetLogoByIdAsync(
        Guid logoId,
        CancellationToken ct = default);
    Task DeleteBrandLogoAsync(
        Guid brandId,
        Guid logoId,
        CancellationToken ct = default);

    Task DeleteAllBrandLogosAsync(
        Guid brandId,
        CancellationToken ct = default);

 
}