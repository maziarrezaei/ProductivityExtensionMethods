#if (NETCOREAPP3_0 || NETCOREAPP3_1 || NETSTANDARD2_1)
#define SUPPORT_NETSTANDARD2_1_AND_ABOVE
#endif

#if (NETCOREAPP3_1 || NETCOREAPP3_0 || NETCOREAPP2_2 || NETCOREAPP2_1)
#define CORE2_1_AND_ABOVE
#endif

using System;
using System.Collections.Generic;

#region Extensions Affecting All Types || Generic IList Extensions || IEnumerable Extensions

public static partial class ProductivityExtensions
{
    private class EqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> _comparer;
        private readonly Func<T, int>? _hash;

        public EqualityComparer(Func<T, T, bool> comparer)
        {
            _comparer = comparer;
        }

        public EqualityComparer(Func<T, T, bool> comparer, Func<T, int>? hash)
        {
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

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

#endregion
