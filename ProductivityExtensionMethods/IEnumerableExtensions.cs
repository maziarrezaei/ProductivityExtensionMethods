#if (NETCOREAPP3_0 || NETCOREAPP3_1 || NETSTANDARD2_1)
#define SUPPORT_NETSTANDARD2_1_AND_ABOVE
#endif

#if (NETCOREAPP3_1 || NETCOREAPP3_0 || NETCOREAPP2_2 || NETCOREAPP2_1)
#define CORE2_1_AND_ABOVE
#endif


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public static partial class ProductivityExtensions
{
    public static bool Contains<TSource>(this IEnumerable<TSource> sources, TSource obj, Func<TSource, TSource, bool> comparer)
    {
        return Contains(sources, obj, comparer, null);
    }

    public static bool Contains<TSource>(this IEnumerable<TSource> sources, TSource obj, Func<TSource, TSource, bool> comparer, Func<TSource, int>? hash)
    {
        IEqualityComparer<TSource> equalityComparer = hash == null ? new EqualityComparer<TSource>(comparer) : new EqualityComparer<TSource>(comparer, hash);

        return sources.Contains(obj, equalityComparer);
    }

    public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, TKey, bool> keyComparer, Func<TKey, int> keyHashCalculator) where TKey : notnull
    {
        IEqualityComparer<TKey> equalityComparer = keyHashCalculator == null ? new EqualityComparer<TKey>(keyComparer) : new EqualityComparer<TKey>(keyComparer, keyHashCalculator);

        return source.ToDictionary(keySelector, elementSelector, equalityComparer);
    }

    /// <summary>
    ///     When doing group by, resulting IEnumerable is inherently a dictionary. This method conviniently converts it to an
    ///     Dictionary.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static Dictionary<TKey, TElement[]> ToDictionary<TKey, TElement>(this IEnumerable<IGrouping<TKey, TElement>> source) where TKey : notnull
    {
        return source.ToDictionary(it => it.Key, it => it.ToArray());
    }

    /// <summary>
    ///     When doing group by, resulting IEnumerable is inherently a dictionary. This method conviniently converts it to an
    ///     Dictionary.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    /// <param name="source"></param>
    /// <param name="keyComparer"></param>
    /// <returns></returns>
    public static Dictionary<TKey, TElement[]> ToDictionary<TKey, TElement>(this IEnumerable<IGrouping<TKey, TElement>> source, Func<TKey, TKey, bool> keyComparer) where TKey : notnull
    {
        return ToDictionary(source, keyComparer, null);
    }

    /// <summary>
    ///     When doing group by, resulting IEnumerable is inherently a dictionary. This method conviniently converts it to an
    ///     Dictionary.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    /// <param name="source"></param>
    /// <param name="keyComparer"></param>
    /// <param name="keyHashCalculator"></param>
    /// <returns></returns>
    public static Dictionary<TKey, TElement[]> ToDictionary<TKey, TElement>(this IEnumerable<IGrouping<TKey, TElement>> source, Func<TKey, TKey, bool> keyComparer, Func<TKey, int>? keyHashCalculator) where TKey : notnull
    {
        IEqualityComparer<TKey> equalityComparer = keyHashCalculator == null ? new EqualityComparer<TKey>(keyComparer) : new EqualityComparer<TKey>(keyComparer, keyHashCalculator);

        return ToDictionary(source, equalityComparer);
    }

    /// <summary>
    ///     When doing group by, resulting IEnumerable is inherently a dictionary. This method conviniently converts it to an
    ///     Dictionary.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    /// <param name="source"></param>
    /// <param name="equalityComparer"></param>
    /// <returns></returns>
    public static Dictionary<TKey, TElement[]> ToDictionary<TKey, TElement>(this IEnumerable<IGrouping<TKey, TElement>> source, IEqualityComparer<TKey> equalityComparer) where TKey : notnull
    {
        IEnumerable<IGrouping<TKey, IGrouping<TKey, TElement>>> uniqueKeyGroupBy = source.GroupBy(grp => grp.Key, equalityComparer);

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
    ///     Converts a flat list of TEntity to a hierarchy of <typeparamref name="TNode" />, optimized for very large number of
    ///     items.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity</typeparam>
    /// <typeparam name="TNode">The type of the node in the hierarchy</typeparam>
    /// <typeparam name="TKey">The type of the identifier</typeparam>
    /// <param name="source">the source list</param>
    /// <param name="convertor">
    ///     The function to use for casting a <typeparamref name="TEntity" /> to a
    ///     <typeparamref name="TNode" />. return null ignore the <typeparamref name="TEntity" />.
    /// </param>
    /// <param name="getKey">
    ///     The function to use for extracting the key from a <typeparamref name="TEntity" />. return null
    ///     ignore the <typeparamref name="TEntity" />.
    /// </param>
    /// <param name="getParentKey">
    ///     The function to use for extracting the parent key of a <typeparamref name="TEntity" />.
    ///     return null to explicitly indicate the <typeparamref name="TEntity" /> as root (performance upside).
    /// </param>
    /// <param name="firstIsParentOfSecond">The function to call whenever a child of a parent node is found.</param>
    /// <remarks>
    ///     The order of the operation is O(n). Implementation performs optimized loop if the instance is a generic List or
    ///     even better an array, but
    ///     do not create new instances of generic list or array to pass in, since the overall performance will be lower.
    /// </remarks>
    /// <returns>the IEnumerable of root <typeparamref name="TNode" /></returns>
    public static IEnumerable<TNode> ToHierarchy<TEntity, TKey, TNode>(this IEnumerable<TEntity> source, Func<TEntity, TNode?> convertor, Func<TEntity, TKey?> getKey, Func<TEntity, TKey?> getParentKey, Action<TNode, TNode> firstIsParentOfSecond) where TKey : struct
                                                                                                                                                                                                                                                      where TNode : class
                                                                                                                                                                                                                                                      where TEntity : class
    {
        return ToHierarchy(source, null, convertor, getKey, getParentKey, firstIsParentOfSecond);
    }

    /// <summary>
    ///     Converts a flat list of TEntity to a hierarchy of <typeparamref name="TNode" />, optimized for very large number of
    ///     items.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity</typeparam>
    /// <typeparam name="TNode">The type of the node in the hierarchy</typeparam>
    /// <typeparam name="TKey">The type of the identifier</typeparam>
    /// <param name="source">the source list.</param>
    /// <param name="keycomparer">
    ///     a comparer to use for compare and get hash code of the entity keys returned by the getKey and
    ///     getParentKey functions
    /// </param>
    /// <param name="convertor">
    ///     The function to use for casting a <typeparamref name="TEntity" /> to a
    ///     <typeparamref name="TNode" />. return null to ignore the <typeparamref name="TEntity" />.
    /// </param>
    /// <param name="getKey">
    ///     The function to use for extracting the key from a <typeparamref name="TEntity" />. return null to
    ///     ignore the <typeparamref name="TEntity" />.
    /// </param>
    /// <param name="getParentKey">
    ///     The function to use for extracting the parent key of a <typeparamref name="TEntity" />.
    ///     return null to explicitly indicate the <typeparamref name="TEntity" /> as root.
    /// </param>
    /// <param name="firstIsParentOfSecond">The function to call whenever a child of a parent node is found.</param>
    /// <remarks>
    ///     The order of the operation is O(n). Implementation performs optimized loop if the instance is a generic List or
    ///     even better an array, but
    ///     do not create new instances of generic list or array to pass in, since the overall performance will be lower.
    /// </remarks>
    /// <returns>the IEnumerable of root <typeparamref name="TNode" /></returns>
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
                foreach (TEntity v in sourceArray)
                    DoConvertToHierarchy(convertor, getKey, getParentKey, firstIsParentOfSecond, v, parents, orphanage, explicitNoParentItems);
                break;
            case List<TEntity> sourceList:
                foreach (TEntity v in sourceList)
                    DoConvertToHierarchy(convertor, getKey, getParentKey, firstIsParentOfSecond, v, parents, orphanage, explicitNoParentItems);
                break;
            default:
                foreach (TEntity v in source)
                    DoConvertToHierarchy(convertor, getKey, getParentKey, firstIsParentOfSecond, v, parents, orphanage, explicitNoParentItems);
                break;
        }

        return (from node in explicitNoParentItems
                select node.Node)
           .Concat(from room in orphanage.Values // There maybe some items that claimed to have parents, but we did not encounter the parent ID. We add them to the root.
                   from node2 in room
                   select node2.Node);
    }

    public static T Max<T>(this IEnumerable<T> enumerable, IComparer<T> comparer)
    {
        if (enumerable == null)
            throw new ArgumentNullException(nameof(enumerable));
        if (comparer == null)
            throw new ArgumentNullException(nameof(comparer));

        return enumerable.Aggregate((max, next) => comparer.Compare(max, next) < 0 ? next : max);
    }

    public static T Min<T>(this IEnumerable<T> enumerable, IComparer<T> comparer)
    {
        if (enumerable == null)
            throw new ArgumentNullException(nameof(enumerable));
        if (comparer == null)
            throw new ArgumentNullException(nameof(comparer));

        return enumerable.Aggregate((min, next) => comparer.Compare(min, next) > 0 ? next : min);
    }

    public static T Max<T>(this IEnumerable<T> enumerable, Func<T, T, int> comparer)
    {
        if (enumerable == null)
            throw new ArgumentNullException(nameof(enumerable));
        if (comparer == null)
            throw new ArgumentNullException(nameof(comparer));

        return enumerable.Aggregate((max, next) => comparer(max, next) < 0 ? next : max);
    }

    public static T Min<T>(this IEnumerable<T> enumerable, Func<T, T, int> comparer)
    {
        if (enumerable == null)
            throw new ArgumentNullException(nameof(enumerable));
        if (comparer == null)
            throw new ArgumentNullException(nameof(comparer));

        return enumerable.Aggregate((min, next) => comparer(min, next) > 0 ? next : min);
    }

#if SUPPORT_NETSTANDARD2_1_AND_ABOVE

    /// <summary>
    /// Picks the first item, and returns the rest without exhausting the enumerable.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">The enumerable source</param>
    /// <param name="pickedValue">The first item in the enumerable. Null if the list is empty.</param>
    /// <returns>The remaining items in the enumerable.</returns>
    public static IEnumerable<T> PickFirst<T>(this IEnumerable<T> source, out T? pickedValue) where T : class
    {
        return PickFirst(source, out pickedValue, it => it);
    }

    /// <summary>
    /// Picks the first item, and returns the rest without exhausting the enumerable.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">The enumerable source</param>
    /// <param name="pickedValue">The first item in the enumerable. Null if the list is empty.</param>
    /// <returns>The remaining items in the enumerable.</returns>
    public static IEnumerable<T> PickFirst<T>(this IEnumerable<T> source, out T? pickedValue) where T : struct
    {
        return PickFirst(source, out pickedValue, it => it);
    }

    /// <summary>
    /// Picks the first item, and returns the rest without exhausting the enumerable, with a transformation applied on first item
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">The enumerable source</param>
    /// <param name="pickedValue">The first item in the enumerable. Null if the list is empty.</param>
    /// <param name="firstElementTranform">Transformation to apply to the first value. Not called if collection is empty.</param>
    /// <returns>The remaining items in the enumerable.</returns>
    public static IEnumerable<T> PickFirst<T, TResult>(this IEnumerable<T> source, out TResult? pickedValue, Func<T, TResult> firstElementTranform) where TResult : class
    {
        var enumerator = source.GetEnumerator();

        if (!enumerator.MoveNext())
        {
            pickedValue = default;
            return source;
        }

        pickedValue = firstElementTranform(enumerator.Current);

        return GetRest();

        IEnumerable<T> GetRest()
        {
            while (enumerator.MoveNext())
                yield return enumerator.Current;
        }
    }

    /// <summary>
    /// Picks the first item, and returns the rest without exhausting the enumerable, with a transformation applied on first item
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">The enumerable source</param>
    /// <param name="pickedValue">The first item in the enumerable. Null if the list is empty.</param>
    /// <param name="firstElementTranform">Transformation to apply to the first value. Not called if collection is empty.</param>
    /// <returns>The remaining items in the enumerable.</returns>
    public static IEnumerable<T> PickFirst<T, TResult>(this IEnumerable<T> source, out TResult? pickedValue, Func<T, TResult> firstElementTranform) where TResult : struct
    {
        var enumerator = source.GetEnumerator();

        if (!enumerator.MoveNext())
        {
            pickedValue = default;
            return source;
        }

        pickedValue = firstElementTranform(enumerator.Current);

        return GetRest();

        IEnumerable<T> GetRest()
        {
            while (enumerator.MoveNext())
                yield return enumerator.Current;
        }
    }
#else

    /// <summary>
    /// Picks the first item, and returns the rest without exhausting the enumerable.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">The enumerable source</param>
    /// <param name="pickedValue">The first item in the enumerable. Null if the list is empty.</param>
    /// <returns>The remaining items in the enumerable.</returns>
    public static IEnumerable<T> PickFirst<T>(this IEnumerable<T> source, out T pickedValue)
    {
        return PickFirst(source, out pickedValue, it => it);
    }

    /// <summary>
    /// Picks the first item, and returns the rest without exhausting the enumerable, with a transformation applied on first item
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">The enumerable source</param>
    /// <param name="pickedValue">The first item in the enumerable. Null if the list is empty.</param>
    /// <param name="firstElementTranform">Transformation to apply to the first value. Not called if collection is empty.</param>
    /// <returns>The remaining items in the enumerable.</returns>
    public static IEnumerable<T> PickFirst<T, TResult>(this IEnumerable<T> source, out TResult pickedValue, Func<T, TResult> firstElementTranform)
    {
        var enumerator = source.GetEnumerator();

        if (!enumerator.MoveNext())
        {
            pickedValue = default;
            return source;
        }

        pickedValue = firstElementTranform(enumerator.Current);

        return GetRest();

        IEnumerable<T> GetRest()
        {
            while (enumerator.MoveNext())
                yield return enumerator.Current;
        }
    }
#endif

    public static bool AreAllEqual<T>(this T[] source)
    {
        for (int i = 1; i < source.Length; i++)
            if (source[1]?.GetHashCode() != source[0]?.GetHashCode() || !Equals(source[i], source[0]))
                return false;

        return true;
    }

    public static bool AreAllEqual<T>(this T[] source, Func<T, T, bool> comparer)
    {
        return AreAllEqual(source, new EqualityComparer<T>(comparer));
    }
    public static bool AreAllEqual<T>(this T[] source, Func<T, T, bool> comparer, Func<T, int> hash)
    {
        return AreAllEqual(source, new EqualityComparer<T>(comparer, hash));
    }

    public static bool AreAllEqual<T>(this T[] source, IEqualityComparer<T> comparer)
    {
        for (int i = 1; i < source.Length; i++)
            if (comparer.GetHashCode(source[i]) != comparer.GetHashCode(source[0]) || !comparer.Equals(source[i], source[0]))
                return false;

        return true;
    }

    public static bool AreAllEqual<T>(this IEnumerable<T> source)
    {
        return AreAllEqual(source, (it1, it2) => Equals(it1, it2));
    }

    public static bool AreAllEqual<T>(this IEnumerable<T> source, Func<T, T, bool> comparer)
    {
        return AreAllEqual(source, new EqualityComparer<T>(comparer));
    }
    public static bool AreAllEqual<T>(this IEnumerable<T> source, Func<T, T, bool> comparer, Func<T, int> hash)
    {
        return AreAllEqual(source, new EqualityComparer<T>(comparer, hash));
    }

    public static bool AreAllEqual<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer)
    {
        return source.PickFirst(out var firstVal, f => (hash: comparer.GetHashCode(f), value: f))
                     .All(it => comparer.GetHashCode(it) == firstVal!.Value.hash && comparer.Equals(it, firstVal.Value.value));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void DoConvertToHierarchy<TEntity, TKey, TNode>(Func<TEntity, TNode?> convertor, Func<TEntity, TKey?> getKey, Func<TEntity, TKey?> getParentKey, Action<TNode, TNode> firstIsParentOfSecond, TEntity v, Dictionary<TKey, TreeNode<TEntity, TKey, TNode>> parents, Dictionary<TKey, List<TreeNode<TEntity, TKey, TNode>>> orphanage, List<TreeNode<TEntity, TKey, TNode>> explicitNoParentItems) where TKey : struct
                                                                                                                                                                                                                                                                                                                                                                                                                   where TNode : class
                                                                                                                                                                                                                                                                                                                                                                                                                   where TEntity : class
    {
        if (v == null)
            return;

        TNode? castedNode = convertor(v);
        if (castedNode == null)
            return;

        var node = new TreeNode<TEntity, TKey, TNode>();

        TKey? nodeKey = getKey(v);
        if (!nodeKey.HasValue)
            return;

        node = new TreeNode<TEntity, TKey, TNode>(castedNode, getParentKey(v), nodeKey.Value);

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
                if (!orphanage.TryGetValue(node.ParentKey.Value, out List<TreeNode<TEntity, TKey, TNode>>? orphanageRoom2))
                {
                    orphanageRoom2 = new List<TreeNode<TEntity, TKey, TNode>>();
                    orphanage.Add(node.ParentKey.Value, orphanageRoom2);
                }

                orphanageRoom2.Add(node);
            }
        }
        else
        {
            explicitNoParentItems.Add(node);
        }

        //checking if there are any orphans whose parent is this new node object.
        if (orphanage.TryGetValue(node.NodeKey, out List<TreeNode<TEntity, TKey, TNode>>? orphanageRoom))
        {
            foreach (TreeNode<TEntity, TKey, TNode> v2 in orphanageRoom)
                firstIsParentOfSecond(node.Node, v2.Node);

            orphanage.Remove(node.NodeKey); //there would be no more orphans for this parent, so removing the orphanage room
        }

        parents.Add(node.NodeKey, node);
    }

    private readonly struct TreeNode<TEntity, TKey, TNode> where TKey : struct
    {
        public readonly TNode Node;

        public readonly TKey NodeKey;
        public readonly TKey? ParentKey;

        public TreeNode(TNode node, TKey? parentKey, TKey nodeKey)
        {
            Node = node;
            ParentKey = parentKey;
            NodeKey = nodeKey;
        }
    }
}
