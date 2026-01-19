namespace ECommerce.Application.Interfaces;

public interface IProductService
{
    Task<(IEnumerable<GetProductDTO> Products, int TotalCount)> GetAllProductsAsync(
        ProductParams productParams, 
        CancellationToken ct = default);
    Task<GetProductDTO> GetProductByIdAsync(int id, CancellationToken ct = default);
    Task AddProductAsync(AddProductDTO productDto, CancellationToken ct = default);
    Task UpdateProductAsync(UpdateProductDTO productDto, CancellationToken ct = default);
    Task DeleteProductAsync(int id, CancellationToken ct = default);
    Task<int> GetTotalCountAsync();
}