using System.Linq.Expressions;
using ChargingStation.Domain.Abstract;

namespace ChargingStation.Infrastructure.Specifications;

public static class ExpressionBuilder<TEntity> where TEntity : Entity
{
    private static readonly ParameterExpression Parameter = Expression.Parameter(typeof(TEntity), "t");
    
    /// <summary>
    /// Creates OrderBy expression of the specified type by dynamic propertyName.
    /// </summary>
    /// <param name="propertyName">Property name.</param>
    /// <returns>Expression like <c>t => t.PropertyName</c>.</returns>
    public static Expression<Func<TEntity, object>> OrderByExpression(string propertyName)
    {
        var propertyReference = Expression.Property(Parameter, propertyName);
        var expression = Expression.Convert(propertyReference, typeof(object));

        return Expression.Lambda<Func<TEntity, object>>(expression, Parameter);
    }
    
    public static Expression<Func<TEntity, bool>> AndAlso(Expression<Func<TEntity, bool>>? left, Expression<Func<TEntity, bool>>? right)
    {
        return CombineFilters(left, right, Expression.AndAlso);
    }
    
    public static Expression<Func<TEntity, bool>> OrElse(Expression<Func<TEntity, bool>>? left, Expression<Func<TEntity, bool>>? right)
    {
        return CombineFilters(left, right, Expression.OrElse);
    }
    
    private static Expression<Func<TEntity, bool>> CombineFilters(Expression<Func<TEntity, bool>>? left, Expression<Func<TEntity, bool>>? right, Func<Expression, Expression, BinaryExpression> combiner)
    {
        if(left is null && right is null)
            throw new ArgumentException("At least one argument must not be null");
        
        if (left == null)
            return right;

        if (right == null)
            return left;

        var parameter = Expression.Parameter(typeof(TEntity));

        var combinedSpecification = combiner.Invoke(
            new ReplaceParameterVisitor { { left.Parameters.Single(), parameter } }.Visit(left.Body),
            new ReplaceParameterVisitor { { right.Parameters.Single(), parameter } }.Visit(right.Body));

        return Expression.Lambda<Func<TEntity, bool>>(combinedSpecification, parameter);
    }
    
    private class ReplaceParameterVisitor : ExpressionVisitor, IEnumerable<KeyValuePair<ParameterExpression, ParameterExpression>>
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> _parameterMappings = new();

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return _parameterMappings.GetValueOrDefault(node, node);
        }

        public void Add(ParameterExpression parameterToReplace, ParameterExpression replaceWith) => _parameterMappings.Add(parameterToReplace, replaceWith);

        public IEnumerator<KeyValuePair<ParameterExpression, ParameterExpression>> GetEnumerator() => _parameterMappings.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
}