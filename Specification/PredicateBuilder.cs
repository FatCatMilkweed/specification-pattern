using System.Linq.Expressions;

namespace Specification;

//https://petemontgomery.wordpress.com/2011/02/10/a-universal-predicatebuilder/
/// <summary>
/// Enables the efficient, dynamic composition of predicates.
/// </summary>
internal static class PredicateBuilder
{
    /// <summary>
    /// Combines the first predicate with the second using the logical "and".
    /// </summary>
    internal static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first,
                                                     Expression<Func<T, bool>> second)
    {
        return first.Compose<Func<T, bool>>(second, Expression.AndAlso);
    }

    /// <summary>
    /// Combines the first predicate with the second using the logical "or".
    /// </summary>
    internal static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first,
                                                  Expression<Func<T, bool>> second)
    {
        return first.Compose<Func<T, bool>>(second, Expression.OrElse);
    }

    /// <summary>
    /// Negates the predicate.
    /// </summary>
    internal static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
    {
        var negated = Expression.Not(expression.Body);
        return Expression.Lambda<Func<T, bool>>(negated, expression.Parameters);
    }

    /// <summary>
    /// Combines the first expression with the second using the specified merge function.
    /// </summary>
    private static Expression<T> Compose<T>(this LambdaExpression first,
                                            LambdaExpression second,
                                            Func<Expression, Expression, Expression> merge)
    {
        // zip parameters (map from parameters of second to parameters of first)
        var map = first.Parameters
            .Select((f,
                     i) => new { f, s = second.Parameters[i] })
            .ToDictionary(p => p.s, p => p.f);

        // replace parameters in the second lambda expression with the parameters in the first
        var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

        // create a merged lambda expression with parameters from the first expression
        return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
    }

    private class ParameterRebinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> _map;

        private ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            _map = map;
        }

        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map,
                                                   Expression expression)
        {
            return new ParameterRebinder(map).Visit(expression);
        }

        protected override Expression VisitParameter(ParameterExpression parameter)
        {
            if (_map.TryGetValue(parameter, out var replacement))
            {
                parameter = replacement;
            }

            return base.VisitParameter(parameter);
        }
    }
}