namespace ECommerce.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<GetProductDTO>> GetAllProductsAsync(ProductParams productParams);
    Task<GetProductDTO> GetProductByIdAsync(int id);
    Task AddProductAsync(AddProductDTO productDto);
    Task AddPhotoAsync(int ProductId, UploadProductPhotoDto productPhotoDTO);
    Task DeletePhotoAsync(int photoId);
    Task UpdateProductAsync(UpdateProductDTO productDTO);
    Task DeleteProductAsync(int id);

    Task<int> GetTotalCountAsync();
}