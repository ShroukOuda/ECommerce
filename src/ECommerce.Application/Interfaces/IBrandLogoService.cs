using ECommerce.Application.DTO.BrandLogos;

namespace ECommerce.Application.Interfaces;

public interface IBrandLogoService
{
    Task<BrandLogoDTO> UploadlogoAsync(
        Guid brandId,
        UploadBrandLogoDTO dto);
    
    Task<IReadOnlyList<BrandLogoDTO>> GetBrandLogosAsync(
        Guid brandId);
    Task<BrandLogoDTO?> GetBrandLogoBySubTypeAsync(
        Guid brandId, 
        ImageSubType subType);
    
    Task<BrandLogoDTO?> GetLogoByIdAsync(
        Guid brandId,
        Guid logoId);
    Task DeleteBrandLogoAsync(
        Guid brandId,
        Guid logoId);

    Task DeleteAllBrandLogosAsync(
        Guid brandId);

 
}