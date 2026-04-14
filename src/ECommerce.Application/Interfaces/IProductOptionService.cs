using ECommerce.Application.DTO.ProductOption;

namespace ECommerce.Application.Interfaces;

public interface IProductOptionService
{
    Task<IEnumerable<GetProductOptionDTO>> GetOptionsByProductIdAsync(Guid productId, CancellationToken ct = default);
    Task<GetProductOptionDTO> GetOptionByIdAsync(Guid id, CancellationToken ct = default);
    Task AddOptionAsync(AddProductOptionDTO dto, CancellationToken ct = default);
    Task UpdateOptionAsync(UpdateProductOptionDTO dto, CancellationToken ct = default);
    Task DeleteOptionAsync(Guid id, CancellationToken ct = default);
    Task AddOptionValueAsync(AddProductOptionValueDTO dto, CancellationToken ct = default);
    Task DeleteOptionValueAsync(Guid id, CancellationToken ct = default);
}
