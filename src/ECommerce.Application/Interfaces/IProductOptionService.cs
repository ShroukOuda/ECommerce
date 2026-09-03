using ECommerce.Application.DTO.ProductOption;

namespace ECommerce.Application.Interfaces;

public interface IProductOptionService
{
    Task<IEnumerable<GetProductOptionDTO>> GetOptionsByProductIdAsync(Guid productId);
    Task<GetProductOptionDTO> GetOptionByIdAsync(Guid id);
    Task<GetProductOptionDTO> AddOptionAsync(AddProductOptionDTO dto);
    Task<GetProductOptionDTO> UpdateOptionAsync(Guid id, UpdateProductOptionDTO dto);
    Task DeleteOptionAsync(Guid id);
    Task<GetProductOptionValueDTO> AddOptionValueAsync(Guid optionId, AddProductOptionValueDTO dto);
    Task DeleteOptionValueAsync(Guid optionId, Guid valueId);
}
