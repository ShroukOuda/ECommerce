using E_Commerece.Core.DTO;
using E_Commerece.Core.Entites.Product;
using E_Commerece.Core.Models;

namespace E_Commerece.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<GetProductDTO>> GetAllProductsAsync(ProductParams productParams);
    Task<GetProductDTO> GetProductByIdAsync(int id);
    Task AddProductAsync(AddProductDTO productDto);
    Task UpdateProductAsync(UpdateProductDTO productDTO);
    Task DeleteProductAsync(int id);

    Task<int> GetTotalCountAsync();
}