namespace ECommerce.Core.Interfaces;

public interface IUnitOfWork
{
    public ICategoryRepository CategoryRepository { get; }
    public IProductRepository ProductRepository { get; }
    public IProductImageRepository ProductImageRepository { get; }
    public ICategoryImageRepository CategoryImageRepository { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}