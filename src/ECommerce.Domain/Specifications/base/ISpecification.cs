using System.Linq.Expressions;
using ECommerce.Domain.Common;

namespace ECommerce.Domain.Specifications.Base;

public interface ISpecification<TEntity, TKey> 
where TEntity : BaseEntity<TKey> 
where TKey : IEquatable<TKey>
{
    Expression<Func<TEntity, bool>>? Criteria { get; }
    
    List<Expression<Func<TEntity, object>>> Includes { get; }
    
    Expression<Func<TEntity, object>>? OrderBy { get; }
    Expression<Func<TEntity, object>>? OrderByDescending { get; }

    int? Take { get; }
    int? Skip { get; }

    bool IsPagingEnabled { get; }
    bool IsNoTrackingEnabled { get; }
}