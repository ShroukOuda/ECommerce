namespace ECommerce.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    
    public ICategoryRepository CategoryRepository { get; }
    public IProductRepository ProductRepository { get; }
    public IProductImageRepository ProductImageRepository { get; }
    public ICategoryImageRepository CategoryImageRepository { get; }
    
    public UnitOfWork(AppDbContext context)
    {
        _context = context; 
        CategoryRepository = new CategoryRepository(_context);
        ProductRepository = new ProductRepository(_context);
        ProductImageRepository = new ProductImageRepository(_context);
        CategoryImageRepository = new CategoryImageRepository(_context);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}