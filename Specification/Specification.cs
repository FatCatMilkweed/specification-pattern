using System.Linq.Expressions;

namespace Specification;

public sealed class Specification<T>
{
    public Specification(Expression<Func<T, bool>> expression)
    {
        Expression = expression ?? throw new ArgumentNullException(nameof(expression));
    }

    public Expression<Func<T, bool>> Expression { get; }

    public bool IsSatisfiedBy(T obj) => Expression.AsFunc()(obj);
    
    public static implicit operator Expression<Func<T, bool>>(Specification<T> specification) =>
        specification.Expression;

    public static implicit operator Func<T, bool>(Specification<T> specification) => specification.Expression.AsFunc();

    //overloaded to use && and || operators
    public static bool operator false(Specification<T> _) => false;


    //overloaded to use && and || operators
    public static bool operator true(Specification<T> _) => false;


    public static Specification<T> operator &(Specification<T> left,
                                              Specification<T> right) =>
        new(left.Expression.And(right.Expression));

    public static Specification<T> operator |(Specification<T> left,
                                              Specification<T> right) =>
        new(left.Expression.Or(right.Expression));

    public static Specification<T> operator !(Specification<T> spec) => new(spec.Expression.Not());
}