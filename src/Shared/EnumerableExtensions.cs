using System.Collections.Generic;
using System.Linq;

namespace OregonTrail.Shared
{
    public static class EnumerableExtensions
    {
        public static SortedSet<T> ToSortedSet<T>(this IEnumerable<T> ts)
        {
            return new SortedSet<T>(ts);
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> ts, params T[] otherTs)
        {
            return Enumerable.Concat(ts, otherTs);
        }

        public static IEnumerable<T> Precat<T>(this IEnumerable<T> ts, params T[] otherTs)
        {
            return Enumerable.Concat(otherTs, ts);
        }
    }
}
