using System.Linq.Expressions;

namespace ECommerce.Domain.Specifications.Base;

public abstract class BaseSpecification<TEntity, TKey> 
: ISpecification<TEntity, TKey> 
where TEntity : BaseEntity<TKey> 
where TKey : IEquatable<TKey>
{
    public Expression<Func<TEntity, bool>>? Criteria { get; private set; }

    public List<Expression<Func<TEntity, object>>> Includes { get; } = new();

    public Expression<Func<TEntity, object>>? OrderBy { get; private set; }
    public Expression<Func<TEntity, object>>? OrderByDescending { get; private set; }

    public int Take { get; private set; }
    public int Skip { get; private set; }
    public bool IsPagingEnabled { get; private set; }
    public bool IsNoTrackingEnabled { get; private set; }


    protected BaseSpecification()
    {
    }
    protected BaseSpecification(Expression<Func<TEntity, bool>>? criteria = null)
    {
        Criteria = criteria;
    }

    protected void AddCriteria(Expression<Func<TEntity, bool>> criteria)
        => Criteria = criteria;

    protected void AddInclude(Expression<Func<TEntity, object>> includeExpression)
        => Includes.Add(includeExpression);

    protected void AddOrderBy(Expression<Func<TEntity, object>> orderByExpression)
        => OrderBy = orderByExpression;

    protected void AddOrderByDescending(Expression<Func<TEntity, object>> orderByDescExpression)
        => OrderByDescending = orderByDescExpression;

    protected void ApplyPaging(int PageSize, int PageNumber)
    {
        Skip = PageSize * (PageNumber - 1);
        Take = PageSize;
        IsPagingEnabled = true;
    }

    protected void AsNoTracking()
    {
        IsNoTrackingEnabled = true;
    }
   

  
}