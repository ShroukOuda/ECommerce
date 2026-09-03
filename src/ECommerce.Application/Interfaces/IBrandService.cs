using ECommerce.Application.DTO.Brand;

namespace ECommerce.Application.Interfaces;

public interface IBrandService
{
    Task<IEnumerable<GetBrandDTO>> GetAllBrandsAsync();
    Task<GetBrandDTO> GetBrandByIdAsync(Guid id);
    Task<GetBrandDTO> AddBrandAsync(AddBrandDTO dto);
    Task<GetBrandDTO> UpdateBrandAsync(Guid id, UpdateBrandDTO dto);
    Task DeleteBrandAsync(Guid id);
}
