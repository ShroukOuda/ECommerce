using E_Commerece.Core.DTO;
using E_Commerece.Core.Entites.Product;

namespace E_Commerece.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<GetProductDTO>> GetAllProductsAsync();
    Task<IEnumerable<GetProductDTO>> GetAllProductsAsync(string? sortBy);
    Task<GetProductDTO> GetProductByIdAsync(int id);
    Task AddProductAsync(AddProductDTO productDto);
    Task UpdateProductAsync(UpdateProductDTO productDTO);
    Task DeleteProductAsync(int id);
}