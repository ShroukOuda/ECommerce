namespace ECommerce.Domain.Interfaces.Repositories;

public interface IUnitOfWork
{
    public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>()
        where TEntity : BaseEntity<TKey>
        where TKey : IEquatable<TKey>;
    public Task<int> SaveChangesAsync();
}