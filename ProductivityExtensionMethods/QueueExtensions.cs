#if (NETCOREAPP3_0 || NETCOREAPP3_1 || NETSTANDARD2_1)
#define SUPPORT_NETSTANDARD2_1_AND_ABOVE
#endif

#if (NETCOREAPP3_1 || NETCOREAPP3_0 || NETCOREAPP2_2 || NETCOREAPP2_1)
#define CORE2_1_AND_ABOVE
#endif

using System.Collections.Generic;

public static partial class ProductivityExtensions
{
    public static void EnqueueAll<T>(this Queue<T> queue, IEnumerable<T> items)
    {
        foreach (T item in items)
            queue.Enqueue(item);
    }
}
