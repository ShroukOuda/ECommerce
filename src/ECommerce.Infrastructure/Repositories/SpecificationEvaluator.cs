using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Common;

namespace ECommerce.Infrastructure.Repositories;

public class SpecificationEvaluator
{
    public static IQueryable<TEntity> GetQuery<TEntity, TKey>(
        IQueryable<TEntity> inputQuery,
        BaseSpecification<TEntity, TKey> specification)
        where TEntity : BaseEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        var query = inputQuery;
        if (specification.Criteria is not null)
        {
            query = query.Where(specification.Criteria);
        }

        if (specification.Includes is not null)
            query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));

        if (specification.OrderBy is not null)
        {
            query = query.OrderBy(specification.OrderBy);
        }

        if (specification.OrderByDescending is not null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        if (specification.IsPagingEnabled)
        {
            query = query.Skip(specification.Skip).Take(specification.Take);
        }
        return query;
    }
}
