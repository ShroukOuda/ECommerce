using ECommerce.Application.DTO.BrandLogos;

namespace ECommerce.Application.Interfaces;

public interface IBrandLogoService
{
    Task<BrandLogoDTO> UploadlogoAsync(
        Guid brandId,
        UploadBrandLogoDTO dto);
    
    Task DeleteBrandLogoAsync(
        Guid brandId,
        Guid logoId);

    Task DeleteAllBrandLogosAsync(
        Guid brandId);

 
}