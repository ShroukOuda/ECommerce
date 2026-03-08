using ECommerce.Application.DTO.Brand;

namespace ECommerce.Application.Interfaces;

public interface IBrandService
{
    Task<IEnumerable<GetBrandDTO>> GetAllBrandsAsync(CancellationToken ct = default);
    Task<GetBrandDTO> GetBrandByIdAsync(int id, CancellationToken ct = default);
    Task AddBrandAsync(AddBrandDTO dto, CancellationToken ct = default);
    Task UpdateBrandAsync(UpdateBrandDTO dto, CancellationToken ct = default);
    Task DeleteBrandAsync(int id, CancellationToken ct = default);
}
