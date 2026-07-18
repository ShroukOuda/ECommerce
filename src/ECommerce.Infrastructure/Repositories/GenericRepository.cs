using System.Linq.Expressions;
using ECommerce.Domain.Common;
using ECommerce.Domain.Specifications.Base;

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
    
    public  async Task<IReadOnlyList<TEntity>> GetAllAsync(
        CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(BaseSpecification<TEntity, TKey> specification)
    {
        return await SpecificationEvaluator
        .GetQuery(_dbSet, specification).ToListAsync();
    }



    public  async Task<TEntity?> GetByIdAsync(
        TKey id,
        CancellationToken ct = default)
    {
        return await _dbSet.FindAsync(new object[] { id },  ct);
    }
    
    public async Task<TEntity?> GetFirstOrDefaultAsync(BaseSpecification<TEntity, TKey> specification)
    {
        return await SpecificationEvaluator.GetQuery(_dbSet, specification).FirstOrDefaultAsync();
    }

    public  async Task AddAsync(TEntity entity, CancellationToken ct = default)
    {
        if (entity is BaseEntity<TKey> baseEntity)
        {
            baseEntity.CreatedAt = DateTime.UtcNow;
            baseEntity.UpdatedAt = DateTime.UtcNow;
        }

        await _dbSet.AddAsync(entity, ct);
    }
    public  async Task AddRangeAsync(
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
    public  void Update(TEntity entity, CancellationToken ct = default)
    {
        if (entity is BaseEntity<TKey> baseEntity)
        {
            baseEntity.UpdatedAt = DateTime.UtcNow;
        }

        _dbSet.Update(entity);
    }
    
    public  void UpdateRange(
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
    
    public  void Delete(TEntity entity, CancellationToken ct = default)
    {
        _dbSet.Remove(entity);
       
    }
    public  void DeleteRange(
        IEnumerable<TEntity> entities,
        CancellationToken ct = default)
    {
        _dbSet.RemoveRange(entities);
        
    }
    
    public  async Task<bool> ExistsAsync(
        BaseSpecification<TEntity, TKey> specification,
        CancellationToken ct = default)
    {
        return await SpecificationEvaluator.GetQuery(_dbSet, specification).AnyAsync();
    }
    
    public  async Task<int> CountAsync(
        BaseSpecification<TEntity, TKey> specification,
        CancellationToken ct = default)
    {
        return await SpecificationEvaluator.GetQuery(_dbSet, specification).CountAsync();
    }
   
  
}
