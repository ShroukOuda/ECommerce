using ECommerce.Application.DTO.ProductVariant;

namespace ECommerce.Application.Interfaces;

public interface IProductVariantService
{
    Task<IEnumerable<GetProductVariantDTO>> GetVariantsByProductIdAsync(Guid productId, CancellationToken ct = default);
    Task<GetProductVariantDTO> GetVariantByIdAsync(Guid id, CancellationToken ct = default);
    Task AddVariantAsync(AddProductVariantDTO dto, CancellationToken ct = default);
    Task UpdateVariantAsync(UpdateProductVariantDTO dto, CancellationToken ct = default);
    Task DeleteVariantAsync(Guid id, CancellationToken ct = default);
}
