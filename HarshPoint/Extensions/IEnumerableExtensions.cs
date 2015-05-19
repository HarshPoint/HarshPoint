using System;
using System.Collections.Generic;
using System.Linq;

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
    }
}
