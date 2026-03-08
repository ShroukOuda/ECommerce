using ECommerce.Application.DTO.ProductVariant;

namespace ECommerce.Application.Interfaces;

public interface IProductVariantService
{
    Task<IEnumerable<GetProductVariantDTO>> GetVariantsByProductIdAsync(int productId, CancellationToken ct = default);
    Task<GetProductVariantDTO> GetVariantByIdAsync(int id, CancellationToken ct = default);
    Task AddVariantAsync(AddProductVariantDTO dto, CancellationToken ct = default);
    Task UpdateVariantAsync(UpdateProductVariantDTO dto, CancellationToken ct = default);
    Task DeleteVariantAsync(int id, CancellationToken ct = default);
}
