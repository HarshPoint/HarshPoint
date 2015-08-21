using System.Collections.Generic;

namespace HarshPoint
{
    public static class HarshKeyValuePair
    {
        public static KeyValuePair<TKey, TValue> Create<TKey, TValue>(
            TKey key,
            TValue value
        )
            => new KeyValuePair<TKey, TValue>(key, value);

        public static KeyValuePair<TKey, TValue> WithKey<TKey, TValue>(
            this KeyValuePair<TKey, TValue> pair,
            TKey key
        )
            => Create(key, pair.Value);

        public static KeyValuePair<TKey, TValue> WithValue<TKey, TValue>(
            this KeyValuePair<TKey, TValue> pair,
            TValue value
        )
            => Create(pair.Key, value);
    }
}
