namespace ECommerce.Application.Interfaces;

public interface IProductService
{
    Task<PaginatedResult<GetProductsDTO>> GetAllProductsAsync(
        ProductSpecParams productSpecParams);
    Task<GetProductDetailsDTO> GetProductByIdAsync(Guid id);
    Task<PaginatedResult<GetProductsDTO>> GetSimilarProductsAsync(Guid productId, PaginationParams paginationParams);
    Task<GetProductDetailsDTO> AddProductAsync(AddProductDTO productDto);
    Task<GetProductDetailsDTO> UpdateProductAsync(Guid productId, UpdateProductDTO productDto);
    Task DeleteProductAsync(Guid id);
    Task<int> GetTotalCountAsync();
}