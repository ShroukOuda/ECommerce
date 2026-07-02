using System.Linq.Expressions;

namespace ECommerce.Domain.Interfaces.Repositories;

public interface IGenericRepository<TEntity, TKey> 
    where TEntity : BaseEntity<TKey> 
    where TKey : IEquatable<TKey>
{
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default);
    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken ct = default);
    
    
    Task AddAsync(TEntity entity, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);
    
    
    void UpdateAsync(TEntity entity, CancellationToken ct = default);
    void UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);
    void DeleteAsync(TEntity entity, CancellationToken ct = default);
    void DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);
    
    
    Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken ct = default);
    Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken ct = default);
    
    
    
}