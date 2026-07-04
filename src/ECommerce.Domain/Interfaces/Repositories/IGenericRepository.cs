using System.Linq.Expressions;
using ECommerce.Domain.Specifications.Base;

namespace ECommerce.Domain.Interfaces.Repositories;

public interface IGenericRepository<TEntity, TKey> 
    where TEntity : BaseEntity<TKey> 
    where TKey : IEquatable<TKey>
{
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<TEntity>> GetAllAsync(BaseSpecification<TEntity, TKey> specification);
    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken ct = default);
    Task<TEntity?> GetFirstOrDefaultAsync(BaseSpecification<TEntity, TKey> specification);
    
    Task AddAsync(TEntity entity, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);
    
    
    void Update(TEntity entity, CancellationToken ct = default);
    void UpdateRange(IEnumerable<TEntity> entities, CancellationToken ct = default);
    void Delete(TEntity entity, CancellationToken ct = default);
    void DeleteRange(IEnumerable<TEntity> entities, CancellationToken ct = default);
    
    
    Task<bool> ExistsAsync(
        BaseSpecification<TEntity, TKey> specification,
        CancellationToken ct = default);
    Task<int> CountAsync(
        BaseSpecification<TEntity, TKey> specification,
        CancellationToken ct = default);
    
    
    
}