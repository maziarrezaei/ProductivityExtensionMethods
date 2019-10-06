using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Collections;
using System.Reflection;

namespace ProductivityExtensionMethods
{
	[global::System.CodeDom.Compiler.GeneratedCode("ProductivityExtensionMethods", "1.0.0")]
	public static partial class ProductivityExtensions
	{
		#region Extensions Affecting All Types
		public static bool In<T>(this T source, params T[] list)
		{
			return list.Contains(source, (IEqualityComparer<T>?/*nullableRef*/)null);
		}
		public static bool In<T>(this T source, Func<T, T, bool>?/*nullableRef*/ comparer, params T[] list)
		{
			if (comparer == null)
				return In(source, (IEqualityComparer<T>?/*nullableRef*/)null, list);

			return list.Contains(source, new EqualityComparer<T>(comparer));
		}
		public static bool In<T>(this T source, IEqualityComparer<T>?/*nullableRef*/ comparer, params T[] list)
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

		#region String and StringBuilder Extenders
		/// <summary>
		/// Executes the String.IsNullOrEmpty on the current string
		/// </summary>
		/// <returns>True if either empty, white space or null</returns>
		public static bool IsBlank(this String str)
		{
			return string.IsNullOrWhiteSpace(str);
		}
		public static string SubstringAfter(this string input, string value)
		{
			int i = input.IndexOf(value);
			if (i == -1)
				return string.Empty;
			return input.Substring(i + value.Length);
		}
		public static string SubstringAfter(this string input, char value)
		{
			int i = input.IndexOf(value);
			if (i == -1)
				return string.Empty;
			return input.Substring(i + 1);
		}
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

		/// <summary>
		/// Method that limits the length of text to a defined length.
		/// </summary>
		/// <param name="source">The source text.</param>
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
		/// <param name="source">The source text.</param>
		/// <param name="maxLength">The maximum limit of the string to return.</param>
		public static string LimitLength(this string source, int maxLength)
		{
			return LimitLength(source, maxLength, false);
		}
		public static void Clear(this StringBuilder stb)
		{
			stb.Remove(0, stb.Length);
		}
		#endregion

		#region Event Extensions
		public static void RaiseEventOnUIThread<TEventArgs>(this EventHandler<TEventArgs> eventHandler, object sender, TEventArgs e)
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
		public static void RaiseEventOnUIThread(this EventHandler eventHandler, object sender, EventArgs e)
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
		private static readonly Dictionary<string, bool> IsAClosedTypeOfCache = new Dictionary<string, bool>();
		public static bool IsAClosedTypeOf(this Type type, Type openType)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			if (openType == null)
				throw new ArgumentNullException(nameof(openType));

			lock (IsAClosedTypeOfCache)
			{
				bool result;
				string key = type.FullName + "," + openType.FullName;
				if (IsAClosedTypeOfCache.TryGetValue(key, out result))
					return result;
				result = true;
				if (openType.IsGenericTypeDefinition || type.ContainsGenericParameters)
				{
					Type[] genericArgsOfType;
					while (true)
					{
						while (!type.IsGenericType)
						{
							type = type.BaseType;
							if (type == null || type == typeof(object))
							{
								result = false;
								break;
							}
						}
						if (!result)
							break;


#pragma warning disable CS8602 // Dereference of a possibly null reference.
						genericArgsOfType = type.GetGenericArguments();
#pragma warning restore CS8602

						try
						{
							result = genericArgsOfType.Length == openType.GetGenericArguments().Length &&
									 openType.MakeGenericType(genericArgsOfType).IsAssignableFrom(type);
							break;
						}
						catch
						{
							result = false;
							break;
						}

					}
				}
				IsAClosedTypeOfCache.Add(key, result);
				return result;
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
		public static int RemoveAllFast<T>(this IList<T> list, Func<T, bool> predicate)
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

			if (!sourceIndex.Between(0, list.Count - 1))
				throw new IndexOutOfRangeException($"{nameof(sourceIndex)} is out of range.");

			if (!destinationIndex.Between(0, list.Count - 1))
				throw new IndexOutOfRangeException($"{nameof(sourceIndex)} is out of range.");

			T item = list[sourceIndex];
			list.RemoveAt(sourceIndex);

			destinationIndex = (destinationIndex > sourceIndex ? destinationIndex - 1 : destinationIndex);

			list.Insert(destinationIndex, item);
		}
		public static int RemoveAll<T>(this IList<T> list, Func<T, bool> predicate)
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
			if (d.CompareTo(min) < 0) return min;
			else if (d.CompareTo(max) > 0) return max;
			else return d;
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

		#region IEnumerable Extension Methods
		public static IEnumerable<T> Append<T>(this IEnumerable<T> enumeration, T item)
		{
			return enumeration.Concat(Enumerable.Repeat(item, 1));
		}
		public static IEnumerable<T> Prepend<T>(this IEnumerable<T> enumeration, T item)
		{
			return Enumerable.Repeat(item, 1).Concat(enumeration);
		}
		public static bool Contains<TSource>(this IEnumerable<TSource> sources, TSource obj, Func<TSource, TSource, bool> comparer)
		{
			return Contains(sources, obj, comparer, null);
		}
		public static bool Contains<TSource>(this IEnumerable<TSource> sources, TSource obj, Func<TSource, TSource, bool> comparer, Func<TSource, int>?/*nullableRef*/ hash)
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
		public static Dictionary<TKey, IEnumerable<TElement>> ToDictionary<TKey, TElement>(this IEnumerable<IGrouping<TKey, TElement>> source)
		{
			return source.ToDictionary(it => it.Key, it => (IEnumerable<TElement>)it);
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
		public static Dictionary<TKey, IEnumerable<TElement>> ToDictionary<TKey, TElement>(this IEnumerable<IGrouping<TKey, TElement>> source, Func<TKey, TKey, bool> keyComparer, Func<TKey, int> keyHashCalculator)
		{
			IEqualityComparer<TKey> equalityComparer = keyHashCalculator == null ? new EqualityComparer<TKey>(keyComparer) : new EqualityComparer<TKey>(keyComparer, keyHashCalculator);

			var uniqueKeyGroupBy = source.GroupBy(grp => grp.Key, equalityComparer);

			return uniqueKeyGroupBy.ToDictionary(it => it.Key, it => it.SelectMany(i => i), equalityComparer);
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
		public static IEnumerable<TNode> ToHierarchy<TEntity, TKey, TNode>(this IEnumerable<TEntity> source, Func<TEntity, TNode?/*nullableRef*/> convertor, Func<TEntity, TKey?> getKey, Func<TEntity, TKey?> getParentKey, Action<TNode, TNode> firstIsParentOfSecond) where TKey : struct
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
		public static IEnumerable<TNode> ToHierarchy<TEntity, TKey, TNode>(this IEnumerable<TEntity> source, IEqualityComparer<TKey>?/*nullableRef*/ keycomparer, Func<TEntity, TNode?/*nullableRef*/> convertor, Func<TEntity, TKey?> getKey, Func<TEntity, TKey?> getParentKey, Action<TNode, TNode> firstIsParentOfSecond) where TKey : struct
																																																																							   where TNode : class
																																																																							   where TEntity : class
		{
			Dictionary<TKey, TreeNode<TEntity, TKey, TNode>> parents =
			source switch
			{
				ICollection<TEntity> collection2 => new Dictionary<TKey, TreeNode<TEntity, TKey, TNode>>(collection2.Count, keycomparer),
				ICollection collection => new Dictionary<TKey, TreeNode<TEntity, TKey, TNode>>(collection.Count, keycomparer),
				_ => new Dictionary<TKey, TreeNode<TEntity, TKey, TNode>>(keycomparer)
			};

			Dictionary<TKey, List<TreeNode<TEntity, TKey, TNode>>> orphanage = new Dictionary<TKey, List<TreeNode<TEntity, TKey, TNode>>>(keycomparer);
			TreeNode<TEntity, TKey, TNode> node, parentNode;
			List<TreeNode<TEntity, TKey, TNode>> explicitNoParentItems = new List<TreeNode<TEntity, TKey, TNode>>();
			List<TreeNode<TEntity, TKey, TNode>> orphanageRoom;
			TNode?/*nullableRef*/ castedNode;
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

				//finding parent of node

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
					.Concat(
					from room in orphanage.Values
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
