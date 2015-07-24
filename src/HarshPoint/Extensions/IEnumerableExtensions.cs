using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace HarshPoint
{
    public static class IEnumerableExtensions
    {
        public static Boolean Any(this IEnumerable sequence)
        {
            if (sequence == null)
            {
                throw Error.ArgumentNull(nameof(sequence));
            }

            var enumerator = sequence.GetEnumerator();

            using (enumerator as IDisposable)
            {
                return enumerator.MoveNext();
            }
        }

        public static TElement FirstOrDefaultByProperty<TElement, TProperty>(
            this IEnumerable<TElement> sequence,
            Func<TElement, TProperty> projection,
            TProperty value,
            IEqualityComparer<TProperty> equalityComparer
        )
        {
            if (sequence == null)
            {
                throw Error.ArgumentNull(nameof(sequence));
            }

            if (projection == null)
            {
                throw Error.ArgumentNull(nameof(projection));
            }

            if (equalityComparer == null)
            {
                equalityComparer = EqualityComparer<TProperty>.Default;
            }

            return sequence.FirstOrDefault(element =>
                equalityComparer.Equals(
                    projection(element),
                    value
                )
            );
        }

        public static async Task<IEnumerable<TResult>> SelectSequentially<TSource, TResult>(
            this IEnumerable<TSource> sequence,
            Func<TSource, Task<TResult>> selector)
        {
            if (sequence == null)
            {
                throw Error.ArgumentNull(nameof(sequence));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

            var source = sequence.ToArray();
            var results = new TResult[source.Length];

            for (int i = 0; i < results.Length; i++)
            {
                results[i] = await selector(source[i]);
            }

            return results;
        }

        public static IImmutableDictionary<TKey, TElement> ToImmutableDictionaryFirstWins<TSource, TKey, TElement>(
            this IEnumerable<TSource> sequence,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            IEqualityComparer<TKey> keyComparer
        )
        {
            if (sequence == null)
            {
                throw Error.ArgumentNull(nameof(sequence));
            }

            if (keySelector == null)
            {
                throw Error.ArgumentNull(nameof(keySelector));
            }

            if (elementSelector == null)
            {
                throw Error.ArgumentNull(nameof(elementSelector));
            }

            var builder = ImmutableDictionary<TKey, TElement>.Empty
                .WithComparers(keyComparer)
                .ToBuilder();

            foreach (var item in sequence)
            {
                var key = keySelector(item);

                if (builder.ContainsKey(key))
                {
                    continue;
                }

                var value = elementSelector(item);
                builder.Add(key, value);
            }

            return builder.ToImmutable();
        }

    }
}
