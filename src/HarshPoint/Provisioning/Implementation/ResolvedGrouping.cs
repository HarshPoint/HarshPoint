using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ResolvedGrouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        private readonly TKey _key;
        private readonly IEnumerable<TElement> _elements;

        public ResolvedGrouping(TKey key, IEnumerable<TElement> elements)
        {
            _key = key;
            _elements = elements ?? new TElement[0];
        }

        public TKey Key => _key;

        public IEnumerator<TElement> GetEnumerator() => _elements.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    internal static class ResolvedGrouping
    {
        public static IGrouping<TKey, TElement> Create<TKey, TElement>(TKey key, IEnumerable<TElement> elements)
        {
            return new ResolvedGrouping<TKey, TElement>(key, elements);
        }
    }
}
