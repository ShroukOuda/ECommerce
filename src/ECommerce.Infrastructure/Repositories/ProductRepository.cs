using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public class ProductRepository : GenericRepository<Product, int>, IProductRepository
{
    private readonly AppDbContext _context;
    
    public ProductRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public override Task<Product?> GetByIdAsync(int id, CancellationToken ct = default)
    {
       return _context.Products
          .Include(p => p.Category)
          .Include(p => p.Photos)
          .AsNoTracking()
          .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<IEnumerable<Product>> GetAllAsync(ProductParams productParams, CancellationToken ct = default)
    {
        productParams.ValidatePrices();
       
        var pageNumber = productParams.PageNumber < 1 ? 1 : productParams.PageNumber;
        var pageSize = productParams.PageSize < 1 ? 6 : productParams.PageSize;
       
        var query = _context.Products
           .Include(p => p.Category)
           .Include(p => p.Photos)
           .AsNoTracking()
           .AsQueryable();
       

        ApplyFilters(query, productParams);
        ApplySorting(query, productParams);
        ApplyPagination(query, productParams);
       
      

        return await query
           .Skip((pageNumber - 1) * pageSize)
           .Take(pageSize)
           .ToListAsync();

    }
    
    private IQueryable<Product> ApplyFilters(
       IQueryable<Product> query,
       ProductParams productParams)
    {
       if (!string.IsNullOrWhiteSpace(productParams.Search))
       {
          string[] searchTerms = productParams.Search.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
          query = query.Where(p => searchTerms.All(
             term => p.Name.ToLower().Contains(term.ToLower()) || 
                     p.Description.ToLower().Contains(term.ToLower())
          ));
       }

       if (productParams.CategoryId.HasValue)
       {
          query = query.Where(p => p.CategoryId == productParams.CategoryId.Value);
       }

       if (productParams.MinPrice.HasValue)
       {
          query = query.Where(p => p.Price >= productParams.MinPrice.Value);
       }

       if (productParams.MaxPrice.HasValue)
       {
          query = query.Where(p => p.Price <= productParams.MaxPrice.Value);
       }

       return query;
    }

    private IQueryable<Product> ApplySorting(
       IQueryable<Product> query,
       ProductParams productParams)
    {
       return productParams.SortBy switch
       {
          ProductSortBy.PriceAsc => query.OrderBy(p => p.Price),
          ProductSortBy.PriceDesc => query.OrderByDescending(p => p.Price),
          ProductSortBy.NameAsc => query.OrderBy(p => p.Name),
          ProductSortBy.NameDesc => query.OrderByDescending(p => p.Name),
          ProductSortBy.Oldest => query.OrderByDescending(p => p.CreatedAt),
          ProductSortBy.Newest => query.OrderBy(p => p.CreatedAt),
          _ => query.OrderByDescending(p => p.CreatedAt)
       };
    }

    private IQueryable<Product> ApplyPagination(
       IQueryable<Product> query,
       ProductParams productParams)
    {
       return query
          .Skip((productParams.PageNumber - 1) * productParams.PageSize)
          .Take(productParams.PageSize);
    }
}