using System;

namespace OregonTrail.Shared
{
    public static class RandomExtensions
    {
        public static T From<T>(this Random r, params T[] ts)
        {
            if (ts.Length == 0)
                return default(T);

            return ts[r.Next(ts.Length)];
        }

        public static double NextDouble(this Random r, double max)
        {
            return r.NextDouble() * max;
        }

        public static bool NextBool(this Random r)
        {
            return r.Next() % 2 == 0;
        }
    }
}
