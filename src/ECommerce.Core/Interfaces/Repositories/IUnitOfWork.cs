namespace ECommerce.Core.Interfaces;

public interface IUnitOfWork
{
    public ICategoryRepository CategoryRepository { get; }
    public IProductRepository ProductRepository { get; }
    public IPhotoRepository PhotoRepository { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}