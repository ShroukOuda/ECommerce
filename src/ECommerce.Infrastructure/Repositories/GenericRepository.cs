using System.Linq.Expressions;
using ECommerce.Core.Common;
using ECommerce.Core.Interfaces.Repositories;

namespace ECommerce.Infrastructure.Repositories;

public class GenericRepository<T, TKey> : IGenericRepository<T, TKey>
    where T : class
    where TKey : IEquatable<TKey> 
{
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _dbSet = context.Set<T>();
    }
    
    public virtual async Task<IReadOnlyList<T>> GetAllAsync(
        CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public virtual async Task<T?> GetByIdAsync(
        TKey id,
        CancellationToken ct = default)
    {
        return await _dbSet.FindAsync(new object[] { id },  ct);
    }
    
    public virtual async Task AddAsync(T entity, CancellationToken ct = default)
    {
        if (entity is BaseEntity<TKey> baseEntity)
        {
            baseEntity.CreatedAt = DateTime.UtcNow;
            baseEntity.UpdatedAt = DateTime.UtcNow;
        }

        await _dbSet.AddAsync(entity, ct);
    }
    public virtual async Task AddRangeAsync(
        IEnumerable<T> entities,
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
    public virtual Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        if (entity is BaseEntity<TKey> baseEntity)
        {
            baseEntity.UpdatedAt = DateTime.UtcNow;
        }

        _dbSet.Update(entity);
        return Task.CompletedTask;
    }
    public virtual Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }
    public virtual Task DeleteRangeAsync(
        IEnumerable<T> entities,
        CancellationToken ct = default)
    {
        _dbSet.RemoveRange(entities);
        return Task.CompletedTask;
    }
    
    public virtual async Task<bool> ExistsAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken ct = default)
    {
        return await _dbSet.AnyAsync(predicate, ct);
    }
    
    public virtual async Task<int> CountAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken ct = default)
    {
        return predicate == null
            ? await _dbSet.CountAsync(ct)
            : await _dbSet.CountAsync(predicate, ct);
    }
   
  
}
