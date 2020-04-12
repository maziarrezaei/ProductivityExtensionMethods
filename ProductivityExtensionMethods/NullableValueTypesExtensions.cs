#if (NETCOREAPP3_0 || NETCOREAPP3_1 || NETSTANDARD2_1)
#define SUPPORT_NETSTANDARD2_1_AND_ABOVE
#endif

#if (NETCOREAPP3_1 || NETCOREAPP3_0 || NETCOREAPP2_2 || NETCOREAPP2_1)
#define CORE2_1_AND_ABOVE
#endif

using System;

public static partial class ProductivityExtensions
{
    public static bool HasValueAndEquals<T>(this T? source, object target) where T : struct
    {
        return source.HasValue && source.Value.Equals(target);
    }

    public static bool IsNullOrDefault<T>(this T? o) where T : struct
    {
        return !o.HasValue || o.Value.Equals(default(T));
    }
}
