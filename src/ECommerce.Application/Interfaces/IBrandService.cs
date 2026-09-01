using ECommerce.Application.DTO.Brand;

namespace ECommerce.Application.Interfaces;

public interface IBrandService
{
    Task<IEnumerable<GetBrandDTO>> GetAllBrandsAsync(CancellationToken ct = default);
    Task<GetBrandDTO> GetBrandByIdAsync(Guid id, CancellationToken ct = default);
    Task<GetBrandDTO> AddBrandAsync(AddBrandDTO dto, CancellationToken ct = default);
    Task<GetBrandDTO> UpdateBrandAsync(Guid id, UpdateBrandDTO dto, CancellationToken ct = default);
    Task DeleteBrandAsync(Guid id, CancellationToken ct = default);
}
