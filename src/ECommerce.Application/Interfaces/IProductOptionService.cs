using ECommerce.Application.DTO.ProductOption;

namespace ECommerce.Application.Interfaces;

public interface IProductOptionService
{
    Task<IEnumerable<GetProductOptionDTO>> GetOptionsByProductIdAsync(Guid productId, CancellationToken ct = default);
    Task<GetProductOptionDTO> GetOptionByIdAsync(Guid id, CancellationToken ct = default);
    Task<GetProductOptionDTO> AddOptionAsync(AddProductOptionDTO dto, CancellationToken ct = default);
    Task<GetProductOptionDTO> UpdateOptionAsync(Guid id, UpdateProductOptionDTO dto, CancellationToken ct = default);
    Task DeleteOptionAsync(Guid id, CancellationToken ct = default);
    Task<GetProductOptionValueDTO> AddOptionValueAsync(Guid optionId, AddProductOptionValueDTO dto, CancellationToken ct = default);
    Task DeleteOptionValueAsync(Guid optionId, Guid valueId, CancellationToken ct = default);
}
