#if (NETCOREAPP3_0 || NETCOREAPP3_1 || NETSTANDARD2_1)
#define SUPPORT_NETSTANDARD2_1_AND_ABOVE
#endif

#if (NETCOREAPP3_1 || NETCOREAPP3_0 || NETCOREAPP2_2 || NETCOREAPP2_1)
#define CORE2_1_AND_ABOVE
#endif

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

public static partial class ProductivityExtensions
{
#if SUPPORT_NETSTANDARD2_1_AND_ABOVE
    public static bool In<T>([AllowNull]this T item, params T[] list)
    {
        return Array.IndexOf(list, item) >= 0;
    }

    public static bool In<T>([AllowNull] this T item, Func<T, T, bool>? comparer, params T[] list)
    {
        if (comparer == null)
            return Array.IndexOf(list, item) >= 0;

        return list.Contains(item, new EqualityComparer<T>(comparer));
    }

    public static bool In<T>([AllowNull] this T item, Func<T, T, bool>? comparer, Func<T, int>? hashCalculator, params T[] list)
    {
        if (comparer == null)
            return Array.IndexOf(list, item) >= 0;

        return list.Contains(item, new EqualityComparer<T>(comparer, hashCalculator));
    }

    public static bool In<T>([AllowNull] this T item, IEqualityComparer<T>? comparer, params T[] list)
    {
        if (comparer == null)
            return Array.IndexOf(list, item) >= 0;

        return list.Contains(item, comparer);
    }

    public static bool Contains<T>(this T[] array, [AllowNull] T item)
    {
        return Array.IndexOf(array, item) >= 0;
    }
#else
    public static bool In<T>(this T item, params T[] list)
    {
        return Array.IndexOf(list, item) >= 0;
    }

    public static bool In<T>(this T item, Func<T, T, bool>? comparer, params T[] list)
    {
        if (comparer == null)
            return Array.IndexOf(list, item) >= 0;

        return list.Contains(item, new EqualityComparer<T>(comparer));
    }

    public static bool In<T>(this T item, Func<T, T, bool>? comparer, Func<T, int>? hashCalculator, params T[] list)
    {
        if (comparer == null)
            return Array.IndexOf(list, item) >= 0;

        return list.Contains(item, new EqualityComparer<T>(comparer, hashCalculator));
    }

    public static bool In<T>(this T item, IEqualityComparer<T>? comparer, params T[] list)
    {
        if (comparer == null)
            return Array.IndexOf(list, item) >= 0;

        return list.Contains(item, comparer);
    }

    public static bool Contains<T>(this T[] array, T item)
    {
        return Array.IndexOf(array, item) >= 0;
    }
#endif

}
