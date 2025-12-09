using E_Commerece.Core.DTO;
using E_Commerece.Core.Entites.Product;
using E_Commerece.Core.Interfaces;
using E_Commerece.Core.Models;
using E_Commerece.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace E_Commerece.Infrastructure.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    private readonly AppDbContext _context;
    
    public ProductRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync(ProductParams productParams)
    {
        productParams.ValidatePrices();
       
        var pageNumber = productParams.PageNumber < 1 ? 1 : productParams.PageNumber;
        var pageSize = productParams.PageSize < 1 ? 6 : productParams.PageSize;
        var query = _context.Products
           .Include(p => p.Category)
           .Include(p => p.Photos)
           .AsNoTracking()
           .AsQueryable();

        if (!string.IsNullOrEmpty(productParams.Search))
        {
           string[] searchTerms = productParams.Search.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
           query = query.Where(p => searchTerms.All(
              term => p.Name.ToLower().Contains(term.ToLower()) || 
                      p.Description.ToLower().Contains(term.ToLower())
                      ));
        }
          
       
        if (productParams.CategoryId.HasValue)
           query = query.Where(p => p.CategoryId == productParams.CategoryId);
       
        if (productParams.MinPrice.HasValue)
           query = query.Where(p => p.Price >=  productParams.MinPrice.Value);
       
        if (productParams.MaxPrice.HasValue)
           query = query.Where(p => p.Price <= productParams.MaxPrice.Value);

        query = productParams.SortBy switch
        {
           ProductSortBy.NameAsc => query.OrderBy(p => p.Name),
           ProductSortBy.NameDesc => query.OrderByDescending(p => p.Name),
           ProductSortBy.PriceAsc => query.OrderBy(p => p.Price),
           ProductSortBy.PriceDesc => query.OrderByDescending(p => p.Price),
           ProductSortBy.Oldest => query.OrderBy(p => p.CreatedAt),
           ProductSortBy.Newest => query.OrderByDescending(p => p.CreatedAt),
           _ => query.OrderBy(p => p.Name)
        };

        return await query
           .Skip((pageNumber - 1) * pageSize)
           .Take(pageSize)
           .ToListAsync();

    }
    
}