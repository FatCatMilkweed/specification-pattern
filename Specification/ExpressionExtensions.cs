using System.Linq.Expressions;

namespace Specification;

internal static class ExpressionExtensions
{
    internal static Func<TIn, TOut> AsFunc<TIn, TOut>(this Expression<Func<TIn, TOut>> expr)
    {
        return CompiledExpressions<TIn, TOut>.AsFunc(expr);
    }
}