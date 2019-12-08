#if (NETCOREAPP3_0 || NETCOREAPP3_1 || NETSTANDARD2_1)
#define SUPPORT_NETSTANDARD2_1_AND_ABOVE
#endif

#if (NETCOREAPP3_1 || NETCOREAPP3_0 || NETCOREAPP2_2 || NETCOREAPP2_1)
#define CORE2_1_AND_ABOVE
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

#nullable enable

namespace ProductivityExtensionMethods
{
    [global::System.CodeDom.Compiler.GeneratedCode("ProductivityExtensionMethods", "VersionPlaceholder{D8B1B561-500C-4086-91AA-0714457205DA}")]
    public static partial class ProductivityExtensions
    {
        #region Extensions Affecting All Types
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
        #endregion

        #region Nullable Value Types Extensions
        public static bool HasValueAndEquals<T>(this T? source, object target) where T : struct
        {
            return source.HasValue && source.Value.Equals(target);
        }
        public static bool IsNullOrDefault<T>(this T? o) where T : struct
        {
            return !o.HasValue || o.Value.Equals(default(T));
        }
        #endregion

        #region String Extensions
#if SUPPORT_NETSTANDARD2_1_AND_ABOVE
        /// <summary>
        /// Executes the String.IsNullOrWhiteSpace on the current string
        /// </summary>
        /// <returns>True if either empty, white space or null</returns>
        public static bool IsBlank([NotNullWhen(false)] this string? str)
        {
            return string.IsNullOrWhiteSpace(str);
        }
        [return: NotNullIfNotNull("defaultValue")]
        public static string? ValueOrDefault(string? value, string? defaultValue)
        {
            return value.IsBlank() ? defaultValue : value;
        }
#else
        /// <summary>
        /// Executes the String.IsNullOrWhiteSpace on the current string
        /// </summary>
        /// <returns>True if either empty, white space or null</returns>
        public static bool IsBlank(this string? str)
        {
            return string.IsNullOrWhiteSpace(str);
        }
        public static string? ValueOrDefault(string? value, string? defaultValue)
        {
            return value.IsBlank() ? defaultValue : value;
        }
#endif
#if (CORE2_1_AND_ABOVE || SUPPORT_NETSTANDARD2_1_AND_ABOVE)
        public static ReadOnlySpan<char> SubstringAfter(this ReadOnlySpan<char> input, ReadOnlySpan<char> value, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            int i = input.IndexOf(value, stringComparison);
            if (i == -1)
                return string.Empty;
            return input.Slice(i + value.Length);
        }
        public static ReadOnlySpan<char> SubstringAfter(this ReadOnlySpan<char> input, char value)
        {
            int i = input.IndexOf(value);
            if (i == -1)
                return string.Empty;
            return input.Slice(i + 1);
        }
#endif
        public static string SubstringAfter(this string input, string value, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            int i = input.IndexOf(value, stringComparison);
            if (i == -1)
                return string.Empty;
            return input.Substring(i + value.Length);
        }
#if SUPPORT_NETSTANDARD2_1_AND_ABOVE
        public static string SubstringAfter(this string input, char value, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            int i = input.IndexOf(value, stringComparison);
            if (i == -1)
                return string.Empty;
            return input.Substring(i + 1);
        }
        public static StringBuilder StringBuilderJoin(this IEnumerable<string> values, string separator)
        {
            return new StringBuilder().AppendJoin(separator, values);
        }
        public static StringBuilder StringBuilderJoin(this string[] values, string separator)
        {
            return new StringBuilder().AppendJoin(separator, values);
        }
#else
        public static string SubstringAfter(this string input, char value)
        {
            int i = input.IndexOf(value);
            if (i == -1)
                return string.Empty;
            return input.Substring(i + 1);
        }
        public static StringBuilder StringBuilderJoin(this IEnumerable<string> values, string separator)
        {
            var result = new StringBuilder();
            foreach (string st in values)
                result.Append(st).Append(separator);

            if (result.Length > 0)
                result.Remove(result.Length - separator.Length, separator.Length);

            return result;
        }
        public static StringBuilder StringBuilderJoin(this string[] values, string separator)
        {
            var result = new StringBuilder();
            foreach (string st in values)
                result.Append(st).Append(separator);

            if (result.Length > 0)
                result.Remove(result.Length - separator.Length, separator.Length);

            return result;
        }
#endif
        public static string SubstringBefore(this string input, string value)
        {
            int i = input.IndexOf(value);
            if (i == -1)
                return string.Empty;
            return input.Substring(0, i);
        }
        public static string SubstringBefore(this string input, char value)
        {
            int i = input.IndexOf(value);
            if (i == -1)
                return string.Empty;
            return input.Substring(0, i);
        }

#if (CORE2_1_AND_ABOVE || SUPPORT_NETSTANDARD2_1_AND_ABOVE)
        /// <summary>
        /// Method that limits the length of text to a defined length.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="maxLength">The maximum limit of the string to return.</param>
        public static ReadOnlySpan<char> LimitLength(this ReadOnlySpan<char> source, int maxLength)
        {
            if (source.Length <= maxLength)
                return source;

            return source.Slice(0, maxLength);
        }
#endif
        /// <summary>
        /// Method that limits the length of text to a defined length.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="maxLength">The maximum limit of the string to return.</param>
        /// <param name="addEllipse">whether or not to add an ellipse to show string is truncated</param>
        public static string LimitLength(this string source, int maxLength, bool addEllipse)
        {
            if (source.Length <= maxLength)
                return source;
            if (maxLength <= 4)
                addEllipse = false;
            if (addEllipse)
                maxLength -= 3;

            string result = source.Substring(0, maxLength);

            if (addEllipse)
                result += "...";
            return result;
        }
        /// <summary>
        /// Method that limits the length of text to a defined length.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="maxLength">The maximum limit of the string to return.</param>
        public static string LimitLength(this string source, int maxLength)
        {
            return LimitLength(source, maxLength, false);
        }
        public static string StringJoin(this string[] values, string separator)
        {
            return string.Join(separator, values);
        }
        public static string StringJoin(this IEnumerable<string> values, string separator)
        {
            return string.Join(separator, values);
        }
        #endregion

        #region Event Extensions
        public static void TryRaiseEventOnUIThread<TEventArgs>(this EventHandler<TEventArgs> eventHandler, object sender, TEventArgs e)
           where TEventArgs : EventArgs
        {
            if (eventHandler != null)
            {
                foreach (EventHandler<TEventArgs> singleCast in eventHandler.GetInvocationList())
                {
                    if (singleCast.Target is ISynchronizeInvoke syncInvoke && syncInvoke.InvokeRequired)
                    {
                        // Invoke the event on the event subscribers thread
                        syncInvoke.Invoke(eventHandler, new object[] { sender, e });
                    }
                    else
                    {
                        // Raise the event on this thread
                        singleCast(sender, e);
                    }
                }
            }
        }
        public static void TryRaiseEventOnUIThread(this EventHandler eventHandler, object sender, EventArgs e)
        {
            if (eventHandler != null)
            {
                foreach (EventHandler singleCast in eventHandler.GetInvocationList())
                {
                    if (singleCast.Target is ISynchronizeInvoke syncInvoke && syncInvoke.InvokeRequired)
                    {
                        // Invoke the event on the event subscribers thread
                        syncInvoke.Invoke(eventHandler, new object[] { sender, e });
                    }
                    else
                    {
                        // Raise the event on this thread
                        singleCast(sender, e);
                    }
                }
            }
        }
        #endregion

        #region Type Extensions
        private static readonly ConcurrentDictionary<(Type closedType, Type openType), Type[]?> IsAClosedTypeOfCache = new ConcurrentDictionary<(Type closedType, Type openType), Type[]?>();
        /// <summary>
        /// Checks if a type is assignable to a certain generic type with no need to specify type parameters. For example, if a type is IDictionary<,>
        /// </summary>
        /// <remarks>
        /// For the purposes of this method, <paramref name="type"/> is considered a closed type of the non-constructed (i.e. open) <paramref name="openGenericType"/>, if an instance of <paramref name="type"/> can be casted to a closed type created from <paramref name="openGenericType"/> with the same type-parameters that was used to construct <paramref name="type"/> or one if its base classes/interfaces.
        /// This is true when a type or one of its base classes implements or inherits from <paramref name="openGenericType"/>. For example, a class that implements generic IList and is a now closed type is considered a closed type of it, since it is assignable to generic IList
        /// Refer to <see cref="https://docs.microsoft.com/en-us/dotnet/framework/reflection-and-codedom/reflection-and-generic-types"/> for detailed definition of open and closed generic types.
        /// </remarks>
        /// <param name="type">Any type can be passed to be examined, however the type must be a closed generic type, or a non generic type.</param>
        /// <param name="openGenericType">A generic type, without any type parameters specified. A rule of thumb for such type is that one should be able to create a closed type of it by only providing a number of type parameters.</param>
        /// <param name="genericTypeParameters">If <paramref name="type"/> is a closed type of <paramref name="openGenericType"/>, the parameters that was used to construct the generic <paramref name="type"/> or one of its base classes/interfaces</param>
        /// <returns>True, If an instance of <paramref name="type"/> can be casted to a closed type created from <paramref name="openGenericType"/> with the same generic type parameters that was used to defining <paramref name="type"/>. Otherwise, false.</returns>
        public static bool IsAClosedTypeOf(this Type type, Type openGenericType, out Type[]? genericTypeParameters)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (openGenericType == null)
                throw new ArgumentNullException(nameof(openGenericType));

            if (type.IsGenericTypeDefinition || type.ContainsGenericParameters)
                throw new ArgumentException($"All generic parameters must be assigned and each generic parameter must be a non generic type or a closed generic type. Supplied type:" + type.ToString(), nameof(type));

            if (!openGenericType.ContainsGenericParameters)
                throw new ArgumentException($"Parameter supplied does not refer to an open generic type definition. Supplied type: {openGenericType} ", nameof(openGenericType));

            if (!openGenericType.IsGenericTypeDefinition)
                throw new ArgumentException($"Parameter supplied does not refer to a generic type definition. Though the type {openGenericType} is an open type, but it not directly closable by suppling a pre-defined list of type parameters."
                                            + "For example, you can never supply type parameters to type.MakeGenericType(...) when the type is typeof(Dictionary<string,List<>>). Such"
                                            + "types can only be created through reflection and no language allows creation of such types using literal type specification such as the example.", nameof(openGenericType));


            var keyToFind = (type, openGenericType);

            genericTypeParameters = IsAClosedTypeOfCache.GetOrAdd(keyToFind, (key, arg) =>
            {
                if (key.openType.FullName == null)
                    throw new ArgumentException("The instance represents a generic type parameter, an array type, pointer type, or byref type based on a type parameter", nameof(openGenericType));

                if (key.openType.IsInterface)
                {
                    Type? ty = key.closedType.GetInterface(key.openType.FullName);

                    if (ty != null)
                        return ty.GetGenericArguments();

                    // The .GetInterface() does not handle simple cases like IDic<string,string> against IDic<,>, so we'll let it go through
                    // regular type check as if it's not an interface
                }

                Type[] genericArgsOfType;
                Type? currentBaseType = key.closedType;

                while (true)
                {
                    while (!currentBaseType.IsGenericType)
                    {
                        currentBaseType = currentBaseType.BaseType;

                        if (currentBaseType == null || currentBaseType == typeof(object))
                            return null;
                    }

                    genericArgsOfType = currentBaseType.GetGenericArguments();

                    if (key.openType.TryMakeGenericType(out Type? newGenType, genericArgsOfType) && newGenType != null && newGenType.IsAssignableFrom(currentBaseType))
                        return genericArgsOfType;

                }
            }, default(object));

            return genericTypeParameters != null;

        }
        /// <summary>
        /// A version of Type.MakeGenericType that does not throw exception. 
        /// </summary>
        /// <param name="genType">The type to construct a generic type from.</param>
        /// <param name="resultType">The constructed generic type.</param>
        /// <param name="typePrms">The type parameters</param>
        /// <returns></returns>
        public static bool TryMakeGenericType(this Type genType, out Type? resultType, params Type[] typePrms)
        {
            resultType = null;
            try
            {
                if (genType.GetGenericArguments().Length != typePrms.Length)
                    return false;

                resultType = genType.MakeGenericType(typePrms);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool HasAttribute<T>(this MemberInfo mi) where T : Attribute
        {
            return Attribute.GetCustomAttribute(mi, typeof(T), true) != null;
        }
        #endregion

        #region Generic IList Extensions
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
        #endregion

        #region Enum Extensions
        public static T ToEnumChecked<T>(this int v) where T : Enum
        {
            if (Enum.IsDefined(typeof(T), v))
                return (T)Enum.ToObject(typeof(T), v);

            throw new InvalidCastException($"Cannot cast value {v} to enum type {typeof(T)}");
        }
        public static bool TryToEnum<T>(this int v, out T e) where T : Enum
        {
            Type t = typeof(T);
            if (!Enum.IsDefined(t, v))
            {
                e = (T)Enum.GetValues(t).GetValue(0);
                return false;
            }
            e = (T)Enum.ToObject(t, v);
            return true;
        }
        #endregion

        #region IComparable Extensions
        public static bool Between<T>(this T actual, T inclusiveLower, T inclusiveUpper) where T : notnull, IComparable<T>
        {
            return Between(actual, inclusiveLower, true, inclusiveUpper, true);
        }
        public static bool Between<T>(this T actual, T lower, bool lowerInclusive, T upper, bool upperInclusive) where T : notnull, IComparable<T>
        {
            int cLower = actual.CompareTo(lower);
            int cUpper = actual.CompareTo(upper);

            return (cLower > 0 || (lowerInclusive && cLower == 0)) && (cUpper < 0 || (upperInclusive && cUpper == 0));
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
            else if (d.CompareTo(max) > 0)
                return max;
            else
                return d;
        }
        #endregion

        #region DateTime Extensions
        public static DateTime SetTime(this DateTime date, TimeSpan time)
        {
            return date.Date.Add(time);
        }
        public static DateTime SetTime(this DateTime date, int? hours = null, int? minutes = null, int? seconds = null, int? milliseconds = null, int? subMsTicks = null)
        {
            var newTime = date.TimeOfDay.SetTime(0, hours, minutes, seconds, milliseconds, subMsTicks);

            return date.Date.Add(newTime);
        }
        public static TimeSpan SetTime(this TimeSpan ts, int? days, int? hours = null, int? minutes = null, int? seconds = null, int? milliseconds = null, int? subMsTicks = null)
        {
            TimeSpan ticks = subMsTicks.HasValue ? TimeSpan.FromTicks(subMsTicks.Value) : ts - TimeSpan.FromMilliseconds(ts.TotalMilliseconds);

            return new TimeSpan(days ?? ts.Days, hours ?? ts.Hours, minutes ?? ts.Minutes, seconds ?? ts.Seconds, milliseconds ?? ts.Milliseconds) + ticks;
        }
        public static DateTime SetDate(this DateTime date, int? year = null, int? month = null, int? day = null)
        {
            return new DateTime(year ?? date.Year, month ?? date.Month, day ?? date.Day).Add(date.TimeOfDay);
        }
        /// <summary>
        /// Gets the first kind of day based on the passed day time.
        /// </summary>
        /// <param name="date">The date</param>
        /// <param name="dayKind">The kind of day</param>
        /// <returns>The DateTime associated with the first kind of day passed.</returns>
        public static DateTime GetFirst(this DateTime date, DayKind dayKind)
        {
            date = date.Date;

            if (dayKind <= DayKind.DayOfMonth)
            {
                DateTime firstDayOfMonth = date.AddDays(1 - date.Day);

                if (dayKind == DayKind.DayOfMonth)
                    return firstDayOfMonth;

                if (firstDayOfMonth.DayOfWeek != (DayOfWeek)dayKind)
                    return firstDayOfMonth.GetNext((DayOfWeek)dayKind);

                return firstDayOfMonth;
            }

            if (dayKind <= DayKind.DayOfYear)
            {
                DateTime firstDayOfYear = date.SetDate(month: 1, day: 1);

                if (dayKind == DayKind.DayOfYear)
                    return firstDayOfYear;

                if (dayKind >= DayKind.SundayOfYear && dayKind <= DayKind.SaturdayOfYear)
                {
                    DayKind inMonthDayKind = (DayKind)((int)dayKind & ~0b01000);

                    return date.GetFirst(inMonthDayKind);
                }
            }

            if (dayKind == DayKind.DayOfWeek)
                return date.DayOfWeek == DayOfWeek.Sunday ? date : date.GetPrevious(DayOfWeek.Sunday);

            throw new ArgumentOutOfRangeException(nameof(dayKind));
        }
        /// <summary>
        /// Gets the last kind of day based on the passed day time.
        /// </summary>
        /// <param name="date">The  date</param>
        /// <returns>The DateTime associated with the last kind of day passed.</returns>
        public static DateTime GetLast(this DateTime date, DayKind dayKind)
        {
            if (dayKind <= DayKind.DayOfMonth)
            {
                int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);

                DateTime lastDayOfMonth = date.GetFirst(DayKind.DayOfMonth).AddDays(daysInMonth - 1);

                if (dayKind == DayKind.DayOfMonth)
                    return lastDayOfMonth;

                int diff = (int)dayKind - (int)lastDayOfMonth.DayOfWeek;

                if (diff > 0)
                    diff -= 7;

                return lastDayOfMonth.AddDays(diff);
            }

            if (dayKind <= DayKind.DayOfYear)
            {
                DateTime lastDayOfYear = date.Date.SetDate(month: 12, day: 31);

                if (dayKind == DayKind.DayOfYear)
                    return lastDayOfYear;

                if (dayKind >= DayKind.SundayOfYear && dayKind <= DayKind.SaturdayOfYear)
                {
                    DayKind inMonthDayKind = (DayKind)((int)dayKind & ~0b01000);

                    return date.GetLast(inMonthDayKind);
                }
            }

            if (dayKind == DayKind.DayOfWeek)
                return date.DayOfWeek == DayOfWeek.Saturday ? date.Date : date.GetNext(DayOfWeek.Saturday);

            throw new ArgumentOutOfRangeException(nameof(dayKind));
        }
        /// <summary>
        /// Gets the DateTime for the first following date that is the given day of the week
        /// </summary>
        /// <param name="date">The date</param>
        /// <param name="dayOfWeek">The following day of the specified DayOfWeek</param>
        public static DateTime GetNext(this DateTime date, DayOfWeek dayOfWeek)
        {
            int diff = dayOfWeek - date.DayOfWeek;

            if (diff <= 0)
                diff += 7;

            return date.Date.AddDays(diff);
        }
        /// <summary>
        /// Gets a DateTime representing the first date following the current date which falls on the given day of the week
        /// </summary>
        /// <param name="date">The date</param>
        /// <param name="dayOfWeek">The following day of the specified DayOfWeek</param>
        public static DateTime GetPrevious(this DateTime date, DayOfWeek dayOfWeek)
        {
            int diff = dayOfWeek - date.DayOfWeek;

            if (diff >= 0)
                diff -= 7;

            return date.Date.AddDays(diff);
        }
        #endregion

        #region Stream Extensions
        /// <summary>
        /// Copies an stream to another stream using the buffer passed.
        /// </summary>
        /// <param name="source">source stream</param>
        /// <param name="destination">destination stream</param>
        /// <param name="buffer">buffer to use during copy</param>
        /// <returns>number of bytes copied</returns>
        public static long CopyTo(this Stream source, Stream destination, byte[] buffer)
        {
            long total = 0;
            int bytesRead;
            while (true)
            {
                bytesRead = source.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                    return total;
                total += bytesRead;
                destination.Write(buffer, 0, bytesRead);
            }
        }
        /// <summary>
        /// Ensures that the specified number of bytes are read from a stream unless it reaches its end.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified
        ///     byte array with the values between offset and (offset + count - 1) replaced
        ///     by the bytes read from the current source.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin storing the data read
        ///     from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>The total number of bytes read into the buffer. This is equal to the
        ///     number of bytes requested or less if the end of the stream has been reached.</returns>
        public static int SafeRead(this Stream source, byte[] buffer, int offset, int count)
        {
            int t;
            int total = 0;
            while (count > 0)
            {
                t = source.Read(buffer, offset, count);
                if (t == 0)
                    break;
                total += t;
                offset += t;
                count -= t;
            }
            return total;
        }
        #endregion

        #region Queue Extensions
        public static void EnqueueAll<T>(this Queue<T> queue, IEnumerable<T> items)
        {
            foreach (var item in items)
                queue.Enqueue(item);
        }
        #endregion

        #region ADO Extensions

        public static IEnumerable<T> AsEnumerable<T>(this T reader, bool disposeReader = true) where T : IDataReader
        {
            try
            {
                while (reader.Read())
                    yield return reader;
            }
            finally
            {
                if (disposeReader)
                    reader.Close();
            }
        }

        #endregion

        #region Func Lambda Calculus Extensions
        public static Func<TData, TResult> Y<TData, TResult>(Func<Func<TData, TResult>, Func<TData, TResult>> f)
        {
            Recursive<TData, TResult> rec = result => data => f(result(result))(data);
            return rec(rec);
        }
        private delegate Func<TData, TResult> Recursive<TData, TResult>(Recursive<TData, TResult> r);
        /// <summary>
        /// Fixes the parameter of a single input. Also when combined with currying, you can fix all the parameters of a multiple parameter
        /// function
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
        #endregion

        #region IEnumerable Extensions
        public static bool Contains<TSource>(this IEnumerable<TSource> sources, TSource obj, Func<TSource, TSource, bool> comparer)
        {
            return Contains(sources, obj, comparer, null);
        }
        public static bool Contains<TSource>(this IEnumerable<TSource> sources, TSource obj, Func<TSource, TSource, bool> comparer, Func<TSource, int>? hash)
        {
            IEqualityComparer<TSource> equalityComparer = hash == null ? new EqualityComparer<TSource>(comparer) : new EqualityComparer<TSource>(comparer, hash);

            return sources.Contains(obj, equalityComparer);
        }
        public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, TKey, bool> keyComparer, Func<TKey, int> keyHashCalculator)
        {
            IEqualityComparer<TKey> equalityComparer = keyHashCalculator == null ? new EqualityComparer<TKey>(keyComparer) : new EqualityComparer<TKey>(keyComparer, keyHashCalculator);

            return source.ToDictionary(keySelector, elementSelector, equalityComparer);
        }
        /// <summary>
        /// When doing group by, resulting IEnumerable is inherently a dictionary. This method conviniently converts it to an Dictionary.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TElement[]> ToDictionary<TKey, TElement>(this IEnumerable<IGrouping<TKey, TElement>> source)
        {
            return source.ToDictionary(it => it.Key, it => it.ToArray());
        }
        /// <summary>
        /// When doing group by, resulting IEnumerable is inherently a dictionary. This method conviniently converts it to an Dictionary.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="source"></param>
        /// <param name="keyComparer"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TElement[]> ToDictionary<TKey, TElement>(this IEnumerable<IGrouping<TKey, TElement>> source, Func<TKey, TKey, bool> keyComparer)
        {
            return ToDictionary(source, keyComparer, null);
        }
        /// <summary>
        /// When doing group by, resulting IEnumerable is inherently a dictionary. This method conviniently converts it to an Dictionary.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="source"></param>
        /// <param name="keyComparer"></param>
        /// <param name="keyHashCalculator"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TElement[]> ToDictionary<TKey, TElement>(this IEnumerable<IGrouping<TKey, TElement>> source, Func<TKey, TKey, bool> keyComparer, Func<TKey, int>? keyHashCalculator)
        {
            IEqualityComparer<TKey> equalityComparer = keyHashCalculator == null ? new EqualityComparer<TKey>(keyComparer) : new EqualityComparer<TKey>(keyComparer, keyHashCalculator);

            return ToDictionary(source, equalityComparer);
        }
        /// <summary>
        /// When doing group by, resulting IEnumerable is inherently a dictionary. This method conviniently converts it to an Dictionary.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="source"></param>
        /// <param name="equalityComparer"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TElement[]> ToDictionary<TKey, TElement>(this IEnumerable<IGrouping<TKey, TElement>> source, IEqualityComparer<TKey> equalityComparer)
        {
            var uniqueKeyGroupBy = source.GroupBy(grp => grp.Key, equalityComparer);

            return uniqueKeyGroupBy.ToDictionary(it => it.Key, it => it.SelectMany(i => i).ToArray(), equalityComparer);
        }
        public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> sources, Func<TSource, TSource, bool> comparer)
        {
            return sources.Distinct(new EqualityComparer<TSource>(comparer));
        }
        public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> sources, Func<TSource, TSource, bool> comparer, Func<TSource, int> hashCalculator)
        {
            return sources.Distinct(new EqualityComparer<TSource>(comparer, hashCalculator));
        }
        /// <summary>
        /// Converts a flat list of TEntity to a hierarchy of <typeparamref name="TNode"/>, optimized for very large number of items.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity</typeparam>
        /// <typeparam name="TNode">The type of the node in the hierarchy</typeparam>
        /// <typeparam name="TKey">The type of the identifier</typeparam>
        /// <param name="source">the source list</param>
        /// <param name="convertor">The function to use for casting a <typeparamref name="TEntity"/> to a <typeparamref name="TNode"/>. return null ignore the <typeparamref name="TEntity"/>.</param>
        /// <param name="getKey">The function to use for extracting the key from a <typeparamref name="TEntity"/>. return null ignore the <typeparamref name="TEntity"/>.</param>
        /// <param name="getParentKey">The function to use for extracting the parent key of a <typeparamref name="TEntity"/>. return null to explicitly indicate the <typeparamref name="TEntity"/> as root (performance upside).</param>
        /// <param name="firstIsParentOfSecond">The function to call whenever a child of a parent node is found.</param>
        /// <remarks>The order of the operation is O(n). Implementation performs optimized loop if the instance is a generic List or even better an array, but
        /// do not create new instances of generic list or array to pass in, since the overall performance will be lower.</remarks>
        /// <returns>the IEnumerable of root <typeparamref name="TNode"/></returns>
        public static IEnumerable<TNode> ToHierarchy<TEntity, TKey, TNode>(this IEnumerable<TEntity> source, Func<TEntity, TNode?> convertor, Func<TEntity, TKey?> getKey, Func<TEntity, TKey?> getParentKey, Action<TNode, TNode> firstIsParentOfSecond) where TKey : struct
                                                                                                                                                                                                                                                      where TNode : class
                                                                                                                                                                                                                                                      where TEntity : class
        {
            return ToHierarchy<TEntity, TKey, TNode>(source, null, convertor, getKey, getParentKey, firstIsParentOfSecond);
        }
        /// <summary>
        /// Converts a flat list of TEntity to a hierarchy of <typeparamref name="TNode"/>, optimized for very large number of items.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity</typeparam>
        /// <typeparam name="TNode">The type of the node in the hierarchy</typeparam>
        /// <typeparam name="TKey">The type of the identifier</typeparam>
        /// <param name="source">the source list.</param>
        /// <param name="keycomparer">a comparer to use for compare and get hash code of the entity keys returned by the getKey and getParentKey functions</param>
        /// <param name="convertor">The function to use for casting a <typeparamref name="TEntity"/> to a <typeparamref name="TNode"/>. return null to ignore the <typeparamref name="TEntity"/>.</param>
        /// <param name="getKey">The function to use for extracting the key from a <typeparamref name="TEntity"/>. return null to ignore the <typeparamref name="TEntity"/>.</param>
        /// <param name="getParentKey">The function to use for extracting the parent key of a <typeparamref name="TEntity"/>. return null to explicitly indicate the <typeparamref name="TEntity"/> as root.</param>
        /// <param name="firstIsParentOfSecond">The function to call whenever a child of a parent node is found.</param>
        /// <remarks>The order of the operation is O(n). Implementation performs optimized loop if the instance is a generic List or even better an array, but
        /// do not create new instances of generic list or array to pass in, since the overall performance will be lower.</remarks>
        /// <returns>the IEnumerable of root <typeparamref name="TNode"/></returns>
        public static IEnumerable<TNode> ToHierarchy<TEntity, TKey, TNode>(this IEnumerable<TEntity> source, IEqualityComparer<TKey>? keycomparer, Func<TEntity, TNode?> convertor, Func<TEntity, TKey?> getKey, Func<TEntity, TKey?> getParentKey, Action<TNode, TNode> firstIsParentOfSecond) where TKey : struct
                                                                                                                                                                                                                                                                                               where TNode : class
                                                                                                                                                                                                                                                                                               where TEntity : class
        {
            Dictionary<TKey, TreeNode<TEntity, TKey, TNode>> parents;

            switch (source)
            {
                case ICollection<TEntity> collection2:
                    parents = new Dictionary<TKey, TreeNode<TEntity, TKey, TNode>>(collection2.Count, keycomparer);
                    break;
                case ICollection collection:
                    parents = new Dictionary<TKey, TreeNode<TEntity, TKey, TNode>>(collection.Count, keycomparer);
                    break;
                default:
                    parents = new Dictionary<TKey, TreeNode<TEntity, TKey, TNode>>(keycomparer);
                    break;
            }



            var explicitNoParentItems = new List<TreeNode<TEntity, TKey, TNode>>();
            var orphanage = new Dictionary<TKey, List<TreeNode<TEntity, TKey, TNode>>>(keycomparer);

            switch (source) // each case is handled differently by compiler for performance reason. Body of each case will be inlined by the compiler
            {
                case TEntity[] sourceArray:
                    foreach (var v in sourceArray)
                        DoConvertToHierarchy<TEntity, TKey, TNode>(convertor, getKey, getParentKey, firstIsParentOfSecond, v, parents, orphanage, explicitNoParentItems);
                    break;
                case List<TEntity> sourceList:
                    foreach (var v in sourceList)
                        DoConvertToHierarchy<TEntity, TKey, TNode>(convertor, getKey, getParentKey, firstIsParentOfSecond, v, parents, orphanage, explicitNoParentItems);
                    break;
                default:
                    foreach (var v in source)
                        DoConvertToHierarchy<TEntity, TKey, TNode>(convertor, getKey, getParentKey, firstIsParentOfSecond, v, parents, orphanage, explicitNoParentItems);
                    break;
            }

            return (from node in explicitNoParentItems select node.Node)
                    .Concat(from room in orphanage.Values  // There maybe some items that claimed to have parents, but we did not encounter the parent ID. We add them to the root.
                            from node2 in room
                            select node2.Node);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DoConvertToHierarchy<TEntity, TKey, TNode>(Func<TEntity, TNode?> convertor, Func<TEntity, TKey?> getKey, Func<TEntity, TKey?> getParentKey, Action<TNode, TNode> firstIsParentOfSecond, TEntity v, Dictionary<TKey, TreeNode<TEntity, TKey, TNode>> parents, Dictionary<TKey, List<TreeNode<TEntity, TKey, TNode>>> orphanage, List<TreeNode<TEntity, TKey, TNode>> explicitNoParentItems) where TKey : struct
                                                                                                                                                                                                                                                                                                                                                                                                          where TNode : class
                                                                                                                                                                                                                                                                                                                                                                                                          where TEntity : class
        {
            List<TreeNode<TEntity, TKey, TNode>> orphanageRoom;

            if (v == null)
                return;

            TNode? castedNode = convertor(v);
            if (castedNode == null)
                return;

            TreeNode<TEntity, TKey, TNode> node = new TreeNode<TEntity, TKey, TNode>();

            node.Node = castedNode;

            TKey? nodeKey = getKey(v);
            if (!nodeKey.HasValue)
                return;

            node.NodeKey = nodeKey.Value;
            node.ParentKey = getParentKey(v);

            //finding parent of the node
            if (node.ParentKey.HasValue)
            {
                TreeNode<TEntity, TKey, TNode> parentNode;

                if (parents.TryGetValue(node.ParentKey.Value, out parentNode))
                {
                    firstIsParentOfSecond(parentNode.Node, node.Node);
                }
                else
                {
                    //poor node :( no parent found! it should go to the orphanage
                    if (!orphanage.TryGetValue(node.ParentKey.Value, out orphanageRoom))
                    {
                        orphanageRoom = new List<TreeNode<TEntity, TKey, TNode>>();
                        orphanage.Add(node.ParentKey.Value, orphanageRoom);
                    }

                    orphanageRoom.Add(node);
                }
            }
            else
                explicitNoParentItems.Add(node);

            //checking if there are any orphans whose parent is this new node object.
            if (orphanage.TryGetValue(node.NodeKey, out orphanageRoom))
            {
                foreach (var v2 in orphanageRoom)
                    firstIsParentOfSecond(node.Node, v2.Node);

                orphanage.Remove(node.NodeKey); //there would be no more orphans for this parent, so removing the orphanage room
            }

            parents.Add(node.NodeKey, node);
        }

        private class TreeNode<TEntity, TKey, TNode> where TKey : struct
        {
            public TKey NodeKey;
            public TKey? ParentKey;
            public TNode Node;
        }
        #endregion

        #region Extensions Affecting All Types || Generic IList Extensions || IEnumerable Extensions
        private class EqualityComparer<T> : IEqualityComparer<T>
        {
            private readonly Func<T, T, bool> _comparer;
            private readonly Func<T, int> _hash;

            public EqualityComparer(Func<T, T, bool> comparer) :
                this(comparer, o => 0)
            {
            }

            public EqualityComparer(Func<T, T, bool> comparer, Func<T, int> hash)
            {
                if (comparer == null)
                    throw new ArgumentNullException("comparer");
                if (hash == null)
                    throw new ArgumentNullException("hash");

                _comparer = comparer;
                _hash = hash;
            }

            public bool Equals(T x, T y)
            {
                return _comparer(x, y);
            }

            public int GetHashCode(T obj)
            {
                if (_hash == null)
                    return 0;
                return _hash(obj);
            }
        }
        #endregion
    }

    #region DateTime Extensions
    public enum DayKind
    {
        SundayOfMonth,
        MondayOfMonth,
        TuesdayOfMonth,
        WednesdayOfMonth,
        ThursdayOfMonth,
        FridayOfMonth,
        SaturdayOfMonth,
        DayOfMonth,
        SundayOfYear = 0b01000,
        MondayOfYear,
        TuesdayOfYear,
        WednesdayOfYear,
        ThursdayOfYear,
        FridayOfYear,
        SaturdayOfYear,
        DayOfYear,
        DayOfWeek
    }
    #endregion
}
