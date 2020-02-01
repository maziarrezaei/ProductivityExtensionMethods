using System;
using System.Collections.Generic;

namespace ProductivityExtensionMethods.Common
{
    #region Extensions Affecting All Types || Generic IList Extensions || IEnumerable Extensions

    internal class EqualityComparer<T> : IEqualityComparer<T>
    {
        #region Fields

        private readonly Func<T, T, bool> _comparer;
        private readonly Func<T, int>? _hash;

        #endregion

        #region Constructors

        public EqualityComparer(Func<T, T, bool> comparer)
        {
            _comparer = comparer;
        }

        public EqualityComparer(Func<T, T, bool> comparer, Func<T, int> hash)
        {
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            if (hash == null)
                throw new ArgumentNullException(nameof(hash));

            _comparer = comparer;
            _hash = hash;
        }

        #endregion

        #region IEqualityComparer<T> Members

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

        #endregion
    }

    #endregion
}