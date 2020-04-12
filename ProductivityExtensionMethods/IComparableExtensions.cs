#if (NETCOREAPP3_0 || NETCOREAPP3_1 || NETSTANDARD2_1)
#define SUPPORT_NETSTANDARD2_1_AND_ABOVE
#endif

#if (NETCOREAPP3_1 || NETCOREAPP3_0 || NETCOREAPP2_2 || NETCOREAPP2_1)
#define CORE2_1_AND_ABOVE
#endif

using System;

public static partial class ProductivityExtensions
{
    public static bool Between<T>(this T actual, T inclusiveLower, T inclusiveUpper) where T : notnull, IComparable<T>
    {
        return Between(actual, inclusiveLower, true, inclusiveUpper, true);
    }

    public static bool Between<T>(this T actual, T lower, bool lowerInclusive, T upper, bool upperInclusive) where T : notnull, IComparable<T>
    {
        int cLower = actual.CompareTo(lower);
        int cUpper = actual.CompareTo(upper);

        return (cLower > 0 || lowerInclusive && cLower == 0) && (cUpper < 0 || upperInclusive && cUpper == 0);
    }

    public static T LimitH<T>(this T value, T maximum) where T : notnull, IComparable
    {
        return value.CompareTo(maximum) < 0 ? value : maximum;
    }

    public static T LimitL<T>(this T value, T minimum) where T : notnull, IComparable
    {
        return value.CompareTo(minimum) < 0 ? minimum : value;
    }

    public static T Limit<T>(this T d, T min, T max) where T : notnull, IComparable
    {
        if (d.CompareTo(min) < 0)
            return min;

        if (d.CompareTo(max) > 0)
            return max;

        return d;
    }
}
