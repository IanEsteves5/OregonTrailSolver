using System.Collections.Generic;

namespace OregonTrail.Shared
{
    public static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default(TValue))
        {
            TValue t;
            if (!dict.TryGetValue(key, out t))
                return defaultValue;

            return t;
        }
    }
}
