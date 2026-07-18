namespace ECommerce.Application.Interfaces;

public interface IProductService
{
    Task<PaginatedResult<GetProductsDTO>> GetAllProductsAsync(
        ProductSpecParams productSpecParams, 
        CancellationToken ct = default);
    Task<GetProductDetailsDTO> GetProductByIdAsync(Guid id, CancellationToken ct = default);
    Task<PaginatedResult<GetProductsDTO>> GetSimilarProductsAsync(Guid productId, PaginationParams paginationParams, CancellationToken ct = default);
    Task AddProductAsync(AddProductDTO productDto, CancellationToken ct = default);
    Task UpdateProductAsync(UpdateProductDTO productDto, CancellationToken ct = default);
    Task DeleteProductAsync(Guid id, CancellationToken ct = default);
    Task<int> GetTotalCountAsync();
}