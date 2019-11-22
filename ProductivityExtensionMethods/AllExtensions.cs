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
using System.Diagnostics.CodeAnalysis;

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
        public static bool IsBlank([NotNullWhen(false)] this String str)
        {
            return string.IsNullOrWhiteSpace(str);
        }
#else
        /// <summary>
        /// Executes the String.IsNullOrWhiteSpace on the current string
        /// </summary>
        /// <returns>True if either empty, white space or null</returns>
        public static bool IsBlank(this String str)
        {
            return string.IsNullOrWhiteSpace(str);
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
#else
        public static string SubstringAfter(this string input, char value)
        {
            int i = input.IndexOf(value);
            if (i == -1)
                return string.Empty;
            return input.Substring(i + 1);
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
        /// For the purposes of this method, <paramref name="constructedGenericType"/> is considered a closed type of the non-constructed (i.e. open) <paramref name="openGenericType"/>, if an instance of <paramref name="constructedGenericType"/> can be casted to a closed type created from <paramref name="openGenericType"/> with the same generic type arguments that was involved with defining type A.
        /// For example, IDictionary<![CDATA[<string,string>]]> is a closed type of IDictionary<![CDATA[<,>]]>.
        /// Also, When a type implements, or inherits from a generic type. For example, a class that implements IDictionary and is a closed type is considered a closed type of, since it is assignable to IDictionary.
        /// Refer to <see cref="https://docs.microsoft.com/en-us/dotnet/framework/reflection-and-codedom/reflection-and-generic-types"/> for detailed definition of open and closed generic types.
        /// </summary>
        /// <param name="constructedGenericType">Any type can be passed to be examined, however the type must be a closed generic type, or a non generic type.</param>
        /// <param name="openGenericType">A generic type, without any type specification as parameters. A rule of thumb for such type is that one should be able to create a closed type of it by only providing a number of type parameters.</param>
        /// <param name="genericTypeParameters">The parameters needed to create a closed type version of <paramref name="openGenericType"/> that an instance of <paramref name="constructedGenericType"/> can be casted to.</param>
        /// <returns>If an instance of <paramref name="constructedGenericType"/> can be casted to a closed type created from <paramref name="openGenericType"/> with the same generic type arguments that was involved with defining <paramref name="constructedGenericType"/></returns>
        public static bool IsAClosedTypeOf(this Type constructedGenericType, Type openGenericType, out Type[]? genericTypeParameters)
        {
            if (constructedGenericType == null)
                throw new ArgumentNullException(nameof(constructedGenericType));

            if (openGenericType == null)
                throw new ArgumentNullException(nameof(openGenericType));

            if (constructedGenericType.IsGenericTypeDefinition || constructedGenericType.ContainsGenericParameters)
                throw new ArgumentException($"All generic parameters must be assigned and each generic parameter must be a non generic type or a closed generic type. Supplied type:" + constructedGenericType.ToString(), nameof(constructedGenericType));

            if (!openGenericType.ContainsGenericParameters)
                throw new ArgumentException($"Parameter supplied does not refer to an open generic type definition. Supplied type: {openGenericType} ", nameof(openGenericType));

            if (!openGenericType.IsGenericTypeDefinition)
                throw new ArgumentException($"Parameter supplied does not refer to a generic type definition. Though the type {openGenericType} is an open type, but it not directly closable by suppling a pre-defined list of type parameters."
                                            + "For example, you can never supply type parameters to type.MakeGenericType(...) when the type is typeof(Dictionary<string,List<>>). Such"
                                            + "types can only be created through reflection and no language allows creation of such types using literal type specification such as the example.", nameof(openGenericType));


            var keyToFind = (constructedGenericType, openGenericType);

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
                foreach (var item in collection)
                    list.Add(item);
        }
        public static void InsertRange<T>(this IList<T> list, int index, IEnumerable<T> collection)
        {
            //List<T> may have a better performance
            if (list is List<T> l)
                l.InsertRange(index, collection);
            else
                foreach (var item in collection)
                    list.Insert(index++, item);
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
        public static DateTime SetTime(this DateTime date, int hours)
        {
            return date.SetTime(hours, null, null, null);
        }
        public static DateTime SetTime(this DateTime date, int hours, int minutes)
        {
            return date.SetTime(hours, minutes, null, null);
        }
        public static DateTime SetTime(this DateTime date, int hours, int minutes, int seconds)
        {
            return date.SetTime(hours, minutes, seconds, null);
        }
        public static DateTime SetTime(this DateTime date, int? hours, int? minutes, int? seconds, int? milliseconds)
        {
            var t = date.TimeOfDay;
            TimeSpan time = new TimeSpan(0, hours ?? t.Hours, minutes ?? t.Minutes, seconds ?? t.Seconds, milliseconds ?? t.Milliseconds);
            return date.SetTime(time);
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
        /// When doing group by, resulting IEnumerable is inherently a dictionary. This method conviniently converts it to an IDictionary.
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
        /// When doing group by, resulting IEnumerable is inherently a dictionary. This method conviniently converts it to an IDictionary.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="source"></param>
        /// <param name="keyComparer"></param>
        /// <param name="keyHashCalculator"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TElement[]> ToDictionary<TKey, TElement>(this IEnumerable<IGrouping<TKey, TElement>> source, Func<TKey, TKey, bool> keyComparer, Func<TKey, int> keyHashCalculator)
        {
            IEqualityComparer<TKey> equalityComparer = keyHashCalculator == null ? new EqualityComparer<TKey>(keyComparer) : new EqualityComparer<TKey>(keyComparer, keyHashCalculator);

            var uniqueKeyGroupBy = source.GroupBy(grp => grp.Key, equalityComparer);

            return uniqueKeyGroupBy.ToDictionary(it => it.Key, it => it.SelectMany(i => i).ToArray(), equalityComparer);
        }


        public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> sources, Func<TSource, TSource, bool> comparer)
        {
            return sources.Distinct(new EqualityComparer<TSource>(comparer));
        }

        public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> sources, Func<TSource, TSource, bool> comparer, Func<TSource, int> hash)
        {
            return sources.Distinct(new EqualityComparer<TSource>(comparer, hash));
        }
        #endregion

        #region IEnumerable to Hierarchy Extensions

        /// <summary>
        /// Converts a flat list of TEntity to a hierarchy of TNodes.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity</typeparam>
        /// <typeparam name="TNode">The type of the node in the hierarchy</typeparam>
        /// <typeparam name="TKey">The type of the identifier</typeparam>
        /// <param name="source">the source list</param>
        /// <param name="convertor">The function to use for casting a TEntity to a TNode. return null ignore the TEntity.</param>
        /// <param name="getKey">The function to use for extracting the key from a TEntity. return null ignore the TEntity.</param>
        /// <param name="getParentKey">The function to use for extracting the parent key of a TEntity. return null to explicitly indicate the TEntity as root (performance upside).</param>
        /// <param name="firstIsParentOfSecond">The function to call whenever a child of a parent node is found.</param>
        /// <remarks>The order of the operation is O(n) (yes, that's right)</remarks>
        /// <returns>the IEnumerable of root TNodes</returns>
        public static IEnumerable<TNode> ToHierarchy<TEntity, TKey, TNode>(this IEnumerable<TEntity> source, Func<TEntity, TNode?> convertor, Func<TEntity, TKey?> getKey, Func<TEntity, TKey?> getParentKey, Action<TNode, TNode> firstIsParentOfSecond) where TKey : struct
                                                                                                                                                                                                                                                      where TNode : class
                                                                                                                                                                                                                                                      where TEntity : class
        {
            return ToHierarchy<TEntity, TKey, TNode>(source, null, convertor, getKey, getParentKey, firstIsParentOfSecond);
        }
        /// <summary>
        /// Converts a flat list of TEntity to a hierarchy of TNodes.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity</typeparam>
        /// <typeparam name="TNode">The type of the node in the hierarchy</typeparam>
        /// <typeparam name="TKey">The type of the identifier</typeparam>
        /// <param name="source">the source list</param>
        /// <param name="keycomparer">a comparer to use for compare and get hash code of the entity keys returned by the getKey and getParentKey functions</param>
        /// <param name="convertor">The function to use for casting a TEntity to a TNode. return null to ignore the TEntity.</param>
        /// <param name="getKey">The function to use for extracting the key from a TEntity. return null to ignore the TEntity.</param>
        /// <param name="getParentKey">The function to use for extracting the parent key of a TEntity. return null to explicitly indicate the TEntity as root.</param>
        /// <param name="firstIsParentOfSecond">The function to call whenever a child of a parent node is found.</param>
        /// <remarks>The order of the operation is O(n) (yes, that's right)</remarks>
        /// <returns>the IEnumerable of root TNodes</returns>
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

            var orphanage = new Dictionary<TKey, List<TreeNode<TEntity, TKey, TNode>>>(keycomparer);

            TreeNode<TEntity, TKey, TNode> node, parentNode;

            var explicitNoParentItems = new List<TreeNode<TEntity, TKey, TNode>>();

            List<TreeNode<TEntity, TKey, TNode>> orphanageRoom;

            TNode? castedNode;

            foreach (var v in source)
            {
                if (v == null)
                    continue;

                castedNode = convertor(v);
                if (castedNode == null)
                    continue;

                node = new TreeNode<TEntity, TKey, TNode>();

                node.Node = castedNode;

                TKey? nodeKey = getKey(v);
                if (!nodeKey.HasValue)
                    continue;

                node.NodeKey = nodeKey.Value;
                node.ParentKey = getParentKey(v);

                //finding parent of the node
                if (node.ParentKey.HasValue)
                {
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

                    orphanage.Remove(node.NodeKey);//there would be no more orphans for this parent, so removing the orphanage room
                }

                parents.Add(node.NodeKey, node);
            }

            return (from node2 in explicitNoParentItems select node2.Node)
                    .Concat(from room in orphanage.Values
                            from node3 in room
                            select node3.Node);
        }

        private class TreeNode<TEntity, TKey, TNode> where TKey : struct
        {
            public TKey NodeKey;
            public TKey? ParentKey;
            public TNode Node;
        }
        #endregion

        public class EqualityComparer<T> : IEqualityComparer<T>
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
    }
}
