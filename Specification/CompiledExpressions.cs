using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace Specification;

internal class CompiledExpressions<TIn, TOut>
{
    private static readonly ConcurrentDictionary<Expression<Func<TIn, TOut>>, Func<TIn, TOut>> Cache = new();

    // We are using cache since Compile() is slow
    internal static Func<TIn, TOut> AsFunc(Expression<Func<TIn, TOut>> expression) => 
        Cache.GetOrAdd(expression, expr => expr.Compile());
}