using ECommerce.Application.DTO.ProductVariant;

namespace ECommerce.Application.Interfaces;

public interface IProductVariantService
{
    Task<IEnumerable<GetProductVariantDTO>> GetVariantsByProductIdAsync(Guid productId, CancellationToken ct = default);
    Task<GetProductVariantDTO> GetVariantByIdAsync(Guid id, CancellationToken ct = default);
    Task<GetProductVariantDTO> AddVariantAsync(AddProductVariantDTO dto, CancellationToken ct = default);
    Task<GetProductVariantDTO> UpdateVariantAsync(Guid id, UpdateProductVariantDTO dto, CancellationToken ct = default);
    Task DeleteVariantAsync(Guid id, CancellationToken ct = default);
    Task<GetProductVariantDTO> GetVariantBySKUAsync(Guid id, string sku, CancellationToken ct = default);
}
