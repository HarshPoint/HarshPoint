using System.Collections.Generic;

namespace HarshPoint
{
    public static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary,
            TKey key
        )
        {
            if (dictionary == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(dictionary));
            }

            var result = default(TValue);
            dictionary.TryGetValue(key, out result);
            return result;
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(DictionaryExtensions));
    }
}
