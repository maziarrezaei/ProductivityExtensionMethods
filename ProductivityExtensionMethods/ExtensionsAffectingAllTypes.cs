﻿#if (NETCOREAPP3_0 || NETCOREAPP3_1 || NETSTANDARD2_1)
#define SUPPORT_NETSTANDARD2_1_AND_ABOVE
#endif

#if (NETCOREAPP3_1 || NETCOREAPP3_0 || NETCOREAPP2_2 || NETCOREAPP2_1)
#define CORE2_1_AND_ABOVE
#endif

using System;
using System.Collections.Generic;
using System.Linq;

public static partial class ProductivityExtensions
{
    public static bool In<T>(this T source, params T[] list)
    {
        return list.Contains(source, (IEqualityComparer<T>?)null);
    }

    public static bool In<T>(this T source, Func<T, T, bool>? comparer, params T[] list)
    {
        if (comparer == null)
            return In(source, (IEqualityComparer<T>?)null, list);

        return list.Contains(source, new EqualityComparer<T>(comparer));
    }

    public static bool In<T>(this T source, IEqualityComparer<T>? comparer, params T[] list)
    {
        if (null == source)
            throw new ArgumentNullException("source");

        if (comparer == null)
            return list.Contains(source);

        return list.Contains(source, comparer);
    }

    public static bool Contains<T>(this T[] array, T item)
    {
        return Array.IndexOf(array, item) >= 0;
    }

}
