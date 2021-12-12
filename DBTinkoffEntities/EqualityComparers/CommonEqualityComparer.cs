using System;
using System.Collections.Generic;
using System.Text;

namespace DBTinkoffEntities.EqualityComparers
{
    public class CommonEqualityComparer<T> : IEqualityComparer<T>
    {
        bool IEqualityComparer<T>.Equals(T x, T y) =>
            x?.Equals(y) ?? false;

        int IEqualityComparer<T>.GetHashCode(T obj) =>
            obj?.GetHashCode() ?? 0;
    }
}
