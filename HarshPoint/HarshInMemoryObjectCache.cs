using System;
using System.Collections.Immutable;
using System.Threading;

namespace HarshPoint
{    
    // shamelessly stolen from http://stackoverflow.com/questions/18367839

    public sealed class HarshInMemoryObjectCache<TKey, TValue>
    {
        private IImmutableDictionary<TKey, TValue> _cache = ImmutableDictionary<TKey, TValue>.Empty;

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            if(valueFactory == null)
            {
                throw Error.ArgumentNull(nameof(valueFactory));
            }

            var newValue = default(TValue);
            var newValueCreated = false;

            while (true)
            {
                var oldCache = _cache;

                TValue value;
                if (oldCache.TryGetValue(key, out value))
                {
                    return value;
                }

                // Value not found; create it if necessary
                if (!newValueCreated)
                {
                    newValue = valueFactory(key);
                    newValueCreated = true;
                }

                // Add the new value to the cache
                var newCache = oldCache.Add(key, newValue);

                if (Interlocked.CompareExchange(ref _cache, newCache, oldCache) == oldCache)
                {
                    // Cache successfully written
                    return newValue;
                }

                // Failed to write the new cache because another thread
                // already changed it; try again.
            }
        }

        public void Clear()
        {
            _cache = _cache.Clear();
        }
    }
}
