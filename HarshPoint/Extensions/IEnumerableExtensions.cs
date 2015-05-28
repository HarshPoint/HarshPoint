using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace HarshPoint
{
    public static class IEnumerableExtensions
    {
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

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
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
    }
}
