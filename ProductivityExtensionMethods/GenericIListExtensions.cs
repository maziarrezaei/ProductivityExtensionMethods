#if (NETCOREAPP3_0 || NETCOREAPP3_1 || NETSTANDARD2_1)
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
    /// <summary>
    /// Fast version of the RemoveAt function. Overwrites the element at the specified index
    /// with the last element in the list, then removes the last element, thus lowering the 
    /// inherent O(n) cost to O(1). Intended to be used on *unordered* lists only.
    /// </summary>
    public static void RemoveAtFast<T>(this IList<T> list, int index)
    {
        if (index < list.Count - 1)
        {
            list[index] = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
        }
        else
            list.RemoveAt(index);
    }

    public static int RemoveAllFast<T>(this IList<T> list, Predicate<T> predicate)
    {
        int counter = 0;
        T item;
        for (int i = 0; i < list.Count;)
        {
            item = list[i];
            if (predicate(item))
            {
                list.RemoveAtFast(i);
                counter++;
            }
            else
                i++;
        }
        return counter;
    }

    public static bool AddRangeUnique<T>(this IList<T> list, IEnumerable<T> objs)
    {
        bool b = false;
        foreach (T it in objs)
            b = AddUnique(list, it) || b;
        return b;
    }

    public static bool AddRangeUnique<T>(this IList<T> list, IEnumerable<T> objs, IEqualityComparer<T> comparer)
    {
        bool b = false;
        foreach (T it in objs)
            b = b || AddUnique(list, it, comparer);
        return b;
    }

    public static bool AddRangeUnique<T>(this IList<T> list, IEnumerable<T> objs, Func<T, T, bool> comparer)
    {
        return AddRangeUnique(list, objs, new EqualityComparer<T>(comparer));
    }

    public static bool AddUnique<T>(this IList<T> list, T obj)
    {
        if (!list.Contains(obj))
        {
            list.Add(obj);
            return true;
        }

        return false;
    }

    public static bool AddUnique<T>(this IList<T> list, T obj, IEqualityComparer<T> comparer)
    {
        if (!list.Contains(obj, comparer))
        {
            list.Add(obj);
            return true;
        }

        return false;
    }

    public static bool AddUnique<T>(this IList<T> list, T obj, Func<T, T, bool> comparer)
    {
        return AddUnique(list, obj, new EqualityComparer<T>(comparer));
    }

    public static bool AddUnique<T>(this IList<T> list, T obj, Func<T, T, bool> comparer, Func<T, int> hash)
    {
        return AddUnique(list, obj, new EqualityComparer<T>(comparer, hash));
    }

    public static void AddRange<T>(this IList<T> list, IEnumerable<T> collection)
    {
        if (list is List<T> l)//List<T> may have a better performance
            l.AddRange(collection);
        else
        {
            switch (collection)
            {
                case T[] array:
                    foreach (var item in array) //5x faster than loop over collection
                        list.Add(item);
                    break;
                case List<T> list2:
                    foreach (var item in list2) //4x faster than loop over collection
                        list.Add(item);
                    break;
                default:
                    foreach (var item in collection)
                        list.Add(item);
                    break;
            }

        }
    }

    public static void InsertRange<T>(this IList<T> list, int index, IEnumerable<T> collection)
    {
        //List<T> may have a better performance
        if (list is List<T> l)
            l.InsertRange(index, collection);
        else
        {
            switch (collection)
            {
                case T[] array:
                    foreach (var item in array) //5x faster than loop over collection
                        list.Insert(index++, item);
                    break;
                case List<T> list2:
                    foreach (var item in list2) //4x faster than loop over collection
                        list.Insert(index++, item);
                    break;
                default:
                    foreach (var item in collection)
                        list.Insert(index++, item);
                    break;
            }
        }
    }

    public static void Move<T>(this IList<T> list, int sourceIndex, int destinationIndex)
    {
        if (sourceIndex == destinationIndex)
            return;

        if (sourceIndex < 0 || sourceIndex > list.Count - 1)
            throw new IndexOutOfRangeException($"{nameof(sourceIndex)} is out of range.");

        if (destinationIndex < 0 || destinationIndex > list.Count - 1)
            throw new IndexOutOfRangeException($"{nameof(sourceIndex)} is out of range.");

        T item = list[sourceIndex];
        list.RemoveAt(sourceIndex);

        destinationIndex = (destinationIndex > sourceIndex ? destinationIndex - 1 : destinationIndex);

        list.Insert(destinationIndex, item);
    }

    public static int RemoveAll<T>(this IList<T> list, Predicate<T> predicate)
    {
        if (list is List<T> l) //List<T> may have a better performance
            return l.RemoveAll(predicate);
        else
        {
            int counter = 0;
            T item;
            for (int i = 0; i < list.Count;)
            {
                item = list[i];
                if (predicate(item))
                {
                    list.RemoveAt(i);
                    counter++;
                }
                else
                    i++;
            }
            return counter;
        }
    }

    public static void AddSorted<T>(this IList<T> list, T item) where T : IComparable<T>
    {
        if (list.Count == 0)
        {
            list.Add(item);
        }
        else if (item.CompareTo(list[list.Count - 1]) >= 0)
        {
            list.Add(item);
        }
        else if (item.CompareTo(list[0]) <= 0)
        {
            list.Insert(0, item);
        }
        else
        {
            // Search for the place to insert.
            int index = BinarySearch(list, 0, list.Count, item);
            if (index < 0)
            {
                // If not found, index is the bitwise complement of the index of the next element larger than item.
                index = ~index;
            }
            list.Insert(index, item);
        }
    }

    /// <summary>
    /// Searches a section of an IList for a given element using a binary search algorithm. Elements of the array are compared to the search value using
    /// the given comparer function. This method assumes that the list is already sorted; if this is not the case, the
    /// result will be incorrect.
    /// 
    /// is larger than the given search value.
    /// </summary>
    /// <param name="list">The list to search in. It should contain sorted elements</param>
    /// <param name="index">The index to start the search from</param>
    /// <param name="length">The length starting the index that the search must be performed</param>
    /// <param name="value">The value to find.</param>
    /// <returns>
    /// The index of the given value in the list. If the list does not contain the given value, the method returns a negative
    /// integer. The bitwise complement operator (~) can be applied to a negative result to produce the index of the first element (if any) that
    /// is larger than the given search value.
    /// </returns>
    public static int BinarySearch<T>(this IList<T> list, int index, int length, T value) where T : IComparable<T>
    {
        if (list == null)
            throw new ArgumentNullException(nameof(list));

        if (index < 0 || length < 0)
            throw new ArgumentOutOfRangeException($"{index} or {length}", "Negative parameter is not acceptable");

        if (list.Count - index < length)
            throw new ArgumentException("The length of search is outside of the list, starting from the given index");

        int lo = index;
        int hi = index + length - 1;

        while (lo <= hi)
        {
            // i might overflow if lo and hi are both large positive numbers. 
            int i = lo + ((hi - lo) >> 1);

            int c = list[i].CompareTo(value);

            if (c == 0)
                return i;

            if (c < 0)
                lo = i + 1;
            else
                hi = i - 1;
        }

        return ~lo;
    }
}
