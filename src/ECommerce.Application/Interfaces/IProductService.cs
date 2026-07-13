namespace ECommerce.Application.Interfaces;

public interface IProductService
{
    Task<PaginatedResult<GetProductDTO>> GetAllProductsAsync(
        ProductSpecParams productSpecParams, 
        CancellationToken ct = default);
    Task<GetProductDTO> GetProductByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<GetProductDTO>> GetFeaturedProductsAsync();
    Task<IReadOnlyList<GetProductDTO>> GetBestSellerProductsAsync();
    Task<IReadOnlyList<GetProductDTO>> GetNewArrivalProductsAsync();
    Task<IReadOnlyList<GetProductDTO>> GetHotDealProductsAsync();
    Task<IReadOnlyList<GetProductDTO>> GetTopRatedProductsAsync();
    Task AddProductAsync(AddProductDTO productDto, CancellationToken ct = default);
    Task UpdateProductAsync(UpdateProductDTO productDto, CancellationToken ct = default);
    Task DeleteProductAsync(Guid id, CancellationToken ct = default);
    Task<int> GetTotalCountAsync();
}