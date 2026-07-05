namespace ECommerce.Application.Interfaces;

public interface IProductService
{
    Task<PaginatedResult<GetProductDTO>> GetAllProductsAsync(
        ProductSpecParams productSpecParams, 
        CancellationToken ct = default);
    Task<GetProductDTO> GetProductByIdAsync(Guid id, CancellationToken ct = default);
    Task AddProductAsync(AddProductDTO productDto, CancellationToken ct = default);
    Task UpdateProductAsync(UpdateProductDTO productDto, CancellationToken ct = default);
    Task DeleteProductAsync(Guid id, CancellationToken ct = default);
    Task<int> GetTotalCountAsync();
}