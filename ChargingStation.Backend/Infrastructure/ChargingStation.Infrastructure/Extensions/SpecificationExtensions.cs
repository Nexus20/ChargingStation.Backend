using ChargingStation.Domain.Abstract;
using ChargingStation.Infrastructure.Specifications;
using Microsoft.EntityFrameworkCore;

namespace ChargingStation.Infrastructure.Extensions;

public static class SpecificationExtensions
{
    public static IQueryable<TEntity> ApplySpecifications<TEntity>(this IQueryable<TEntity> query,
        Specification<TEntity> specification) where TEntity : Entity
    {
        if(specification.Filter is not null)
            query = query.Where(specification.Filter);
        
        if(specification.Includes.Count != 0)
            query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));
        
        if (specification.OrderBy != null)
        {
            if (specification.IsDescendingOrderBy.HasValue && specification.IsDescendingOrderBy.Value)
            {
                if (specification.ThenBy != null)
                {
                    query = specification.IsDescendingThenBy.HasValue && specification.IsDescendingThenBy.Value
                        ? query.OrderByDescending(specification.OrderBy).ThenByDescending(specification.ThenBy)
                        : query.OrderByDescending(specification.OrderBy).ThenBy(specification.ThenBy);
                }
                else
                {
                    query = query.OrderByDescending(specification.OrderBy);
                }
            }
            else
            {
                if (specification.ThenBy != null)
                {
                    query = specification.IsDescendingThenBy.HasValue && specification.IsDescendingThenBy.Value
                        ? query.OrderBy(specification.OrderBy).ThenByDescending(specification.ThenBy)
                        : query.OrderBy(specification.OrderBy).ThenBy(specification.ThenBy);
                }
                else
                {
                    query = query.OrderBy(specification.OrderBy);
                }
            }
        }

        return query;
    }
}