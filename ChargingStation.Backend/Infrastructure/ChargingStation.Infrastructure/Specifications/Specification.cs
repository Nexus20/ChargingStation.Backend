using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using ChargingStation.Common.Models.Requests;
using ChargingStation.Domain.Abstract;

namespace ChargingStation.Infrastructure.Specifications;

public abstract class Specification<TEntity> where TEntity : Entity
{
    public Expression<Func<TEntity, bool>>? Filter { get; private set; }
    
    public List<string> Includes { get; } = [];
    
    public Expression<Func<TEntity, object>>? OrderBy { get; private set; }
    public Expression<Func<TEntity, object>>? ThenBy { get; private set; }
    public bool? IsDescendingOrderBy { get; private set; }
    public bool? IsDescendingThenBy { get;  private set;}
    
    protected void AddFilter(Expression<Func<TEntity, bool>> filterExpression)
    {
        if(Filter is not null)
            Filter = ExpressionBuilder<TEntity>.AndAlso(Filter, filterExpression);
        else
            Filter = filterExpression;
        
        Filter = filterExpression;
    }

    protected void AddInclude(string includeString) => Includes.Add(includeString);
    
    private void AddOrderBy(Expression<Func<TEntity, object>> orderByExpression, bool isDescending = false)
    {
        OrderBy = orderByExpression;
        IsDescendingOrderBy = isDescending;
    }

    private void AddThenBy(Expression<Func<TEntity, object>> thenByExpression, bool isDescending = false)
    {
        ThenBy = thenByExpression;
        IsDescendingThenBy = isDescending;
    }
    
    private readonly ConcurrentDictionary<string, PropertyInfo> DiscoveredProperties = new();

    protected void AddSorting(IEnumerable<OrderPredicate> orderPredicates)
    {
        var significantPredicates = orderPredicates
            .Where(x => !string.IsNullOrEmpty(x.PropertyName))
            .Distinct();

        foreach (var predicate in significantPredicates)
        {
            var property = DiscoverProperty(predicate.PropertyName);

            if (!property.PropertyType.IsValueType && property.PropertyType != typeof(string))
                throw new NotSupportedException("Property type is not supported.");

            if (OrderBy is null)
                AddOrderBy(property.Name, predicate.OrderDirection);
            else
                AddThenBy(property.Name, predicate.OrderDirection);
        }
    }

    private PropertyInfo DiscoverProperty(string propertyName)
    {
        if (DiscoveredProperties.TryGetValue(propertyName, out var property))
            return property;

        property = typeof(TEntity).GetProperty(
            propertyName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);

        if (property == null)
            throw new InvalidOperationException("Invalid property name.");

        DiscoveredProperties.TryAdd(propertyName, property);

        return property;
    }

    private void AddOrderBy(string propertyName, OrderDirection option)
    {
        var expression = ExpressionBuilder<TEntity>.OrderByExpression(propertyName);
        
        switch (option)
        {
            case OrderDirection.Descending:
                AddOrderBy(expression, true);
                break;
            case OrderDirection.Ascending:
                AddOrderBy(expression);
                break;
            default:
                throw new NotSupportedException("Order direction is not supported.");
        }
    }

    private void AddThenBy(string propertyName, OrderDirection option)
    {
        var expression = ExpressionBuilder<TEntity>.OrderByExpression(propertyName);
        
        switch (option)
        {
            case OrderDirection.Descending:
                AddThenBy(expression, true);
                break;
            case OrderDirection.Ascending:
                AddThenBy(expression);
                break;
            default:
                throw new NotSupportedException("Order direction is not supported.");
        }
    }
}