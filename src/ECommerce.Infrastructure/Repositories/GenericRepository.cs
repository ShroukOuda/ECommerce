using System.Linq.Expressions;
using ECommerce.Domain.Common;

namespace ECommerce.Infrastructure.Repositories;

public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey>
    where TEntity : BaseEntity<TKey>
    where TKey : IEquatable<TKey> 
{
    protected readonly DbSet<TEntity> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _dbSet = context.Set<TEntity>();
    }
    
    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(
        CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public virtual async Task<TEntity?> GetByIdAsync(
        TKey id,
        CancellationToken ct = default)
    {
        return await _dbSet.FindAsync(new object[] { id },  ct);
    }
    
    public virtual async Task AddAsync(TEntity entity, CancellationToken ct = default)
    {
        if (entity is BaseEntity<TKey> baseEntity)
        {
            baseEntity.CreatedAt = DateTime.UtcNow;
            baseEntity.UpdatedAt = DateTime.UtcNow;
        }

        await _dbSet.AddAsync(entity, ct);
    }
    public virtual async Task AddRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        foreach (var entity in entities)
        {
            if (entity is BaseEntity<TKey> baseEntity)
            {
                baseEntity.CreatedAt = now;
                baseEntity.UpdatedAt = now;
            }
        }

        await _dbSet.AddRangeAsync(entities, ct);
    }
    public virtual void UpdateAsync(TEntity entity, CancellationToken ct = default)
    {
        if (entity is BaseEntity<TKey> baseEntity)
        {
            baseEntity.UpdatedAt = DateTime.UtcNow;
        }

        _dbSet.Update(entity);
    }
    
    public virtual void UpdateRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        foreach (var entity in entities)
        {
            if (entity is BaseEntity<TKey> baseEntity)
            {
                baseEntity.UpdatedAt = now;
            }
        }

        _dbSet.UpdateRange(entities);
        
    }
    
    public virtual void DeleteAsync(TEntity entity, CancellationToken ct = default)
    {
        _dbSet.Remove(entity);
       
    }
    public virtual void DeleteRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken ct = default)
    {
        _dbSet.RemoveRange(entities);
        
    }
    
    public virtual async Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken ct = default)
    {
        return await _dbSet.AnyAsync(predicate, ct);
    }
    
    public virtual async Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken ct = default)
    {
        return predicate == null
            ? await _dbSet.CountAsync(ct)
            : await _dbSet.CountAsync(predicate, ct);
    }
   
  
}
