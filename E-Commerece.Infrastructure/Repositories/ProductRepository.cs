using E_Commerece.Core.DTO;
using E_Commerece.Core.Entites.Product;
using E_Commerece.Core.Interfaces;
using E_Commerece.Infrastructure.Data;

namespace E_Commerece.Infrastructure.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    
    public ProductRepository(AppDbContext context) : base(context)
    {
        
    }
    
}