#if (NETCOREAPP3_0 || NETCOREAPP3_1 || NETSTANDARD2_1)
#define SUPPORT_NETSTANDARD2_1_AND_ABOVE
#endif

#if (NETCOREAPP3_1 || NETCOREAPP3_0 || NETCOREAPP2_2 || NETCOREAPP2_1)
#define CORE2_1_AND_ABOVE
#endif

using System;

public static partial class ProductivityExtensions
{
    public static Func<TData, TResult> Y<TData, TResult>(Func<Func<TData, TResult>, Func<TData, TResult>> f)
    {
        Recursive<TData, TResult> rec = result => data => f(result(result))(data);
        return rec(rec);
    }

    /// <summary>
    ///     Fixes the parameter of a single input. Also when combined with currying, you can fix all the parameters of a
    ///     multiple parameter
    ///     function
    /// </summary>
    public static Func<R> Fix<A, R>(this Func<A, R> f, A parameter)
    {
        return () => f(parameter);
    }

    public static Func<A, Func<B, R>> Curry<A, B, R>(this Func<A, B, R> f)
    {
        return a => b => f(a, b);
    }

    public static Func<A, Func<B, Func<C, R>>> Curry<A, B, C, R>(this Func<A, B, C, R> f)
    {
        return a => b => c => f(a, b, c);
    }

    public static Func<A, Func<B, Func<C, Func<D, R>>>> Curry<A, B, C, D, R>(this Func<A, B, C, D, R> f)
    {
        return a => b => c => d => f(a, b, c, d);
    }

    public static Func<A, B, R> UnCurry<A, B, R>(this Func<A, Func<B, R>> f)
    {
        return (a, b) => f(a)(b);
    }

    public static Func<A, B, C, R> UnCurry<A, B, C, R>(this Func<A, Func<B, Func<C, R>>> f)
    {
        return (a, b, c) => f(a)(b)(c);
    }

    public static Func<A, B, C, D, R> UnCurry<A, B, C, D, R>(this Func<A, Func<B, Func<C, Func<D, R>>>> f)
    {
        return (a, b, c, d) => f(a)(b)(c)(d);
    }

    private delegate Func<TData, TResult> Recursive<TData, TResult>(Recursive<TData, TResult> r);
}
