using System.Linq.Expressions;
using ECommerce.Domain.Specifications.Base;

namespace ECommerce.Domain.Interfaces.Repositories;

public interface IGenericRepository<TEntity, TKey> 
    where TEntity : BaseEntity<TKey> 
    where TKey : IEquatable<TKey>
{
    Task<IReadOnlyList<TEntity>> GetAllAsync();
    Task<IReadOnlyList<TEntity>> GetAllAsync(BaseSpecification<TEntity, TKey> specification);
    Task<TEntity?> GetByIdAsync(TKey id);
    Task<TEntity?> GetFirstOrDefaultAsync(BaseSpecification<TEntity, TKey> specification);
    
    Task AddAsync(TEntity entity);
    Task AddRangeAsync(IEnumerable<TEntity> entities);
    
    
    void Update(TEntity entity);
    void UpdateRange(IEnumerable<TEntity> entities);
    void Delete(TEntity entity);
    void DeleteRange(IEnumerable<TEntity> entities);
    
    
    Task<bool> ExistsAsync(
        BaseSpecification<TEntity, TKey> specification);
    Task<int> CountAsync(
        BaseSpecification<TEntity, TKey> specification);
    
    
    
}