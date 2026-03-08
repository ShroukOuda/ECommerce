using ECommerce.Application.DTO.ProductOption;

namespace ECommerce.Application.Interfaces;

public interface IProductOptionService
{
    Task<IEnumerable<GetProductOptionDTO>> GetOptionsByProductIdAsync(int productId, CancellationToken ct = default);
    Task<GetProductOptionDTO> GetOptionByIdAsync(int id, CancellationToken ct = default);
    Task AddOptionAsync(AddProductOptionDTO dto, CancellationToken ct = default);
    Task UpdateOptionAsync(UpdateProductOptionDTO dto, CancellationToken ct = default);
    Task DeleteOptionAsync(int id, CancellationToken ct = default);
    Task AddOptionValueAsync(AddProductOptionValueDTO dto, CancellationToken ct = default);
    Task DeleteOptionValueAsync(int id, CancellationToken ct = default);
}
