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
}