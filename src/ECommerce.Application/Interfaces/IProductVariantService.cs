using ECommerce.Application.DTO.ProductVariant;

namespace ECommerce.Application.Interfaces;

public interface IProductVariantService
{
    Task<IEnumerable<GetProductVariantDTO>> GetVariantsByProductIdAsync(Guid productId);
    Task<GetProductVariantDTO> GetVariantByIdAsync(Guid id);
    Task<GetProductVariantDTO> AddVariantAsync(AddProductVariantDTO dto);
    Task<GetProductVariantDTO> UpdateVariantAsync(Guid id, UpdateProductVariantDTO dto);
    Task DeleteVariantAsync(Guid id);
    Task<GetProductVariantDTO> GetVariantBySKUAsync(Guid id, string sku);
}
