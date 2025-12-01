using E_Commerece.Core.DTO;
using E_Commerece.Core.Entites.Product;
using E_Commerece.Core.Interfaces;
using E_Commerece.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace E_Commerece.Infrastructure.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    private readonly AppDbContext _context;
    
    public ProductRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync(string? sortBy)
    {
       var query = _context.Products
           .Include(p => p.Category)
           .Include(p => p.Photos)
           .AsNoTracking();

     
        switch (sortBy)
           {
               case "PriceAsc":
                   query = query.OrderBy(p => p.Price);
                   break;
               case "PriceDesc":
                   query = query.OrderByDescending(p => p.Price);
                   break;
               default:
                   query = query.OrderBy(p => p.Name);
                   break;
           }
       
       
       return await query.ToListAsync();
       
    }
    
}