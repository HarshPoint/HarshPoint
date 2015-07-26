using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HarshPoint
{
    internal sealed class HarshGrouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        private readonly TKey _key;
        private readonly IEnumerable<TElement> _elements;

        public HarshGrouping(TKey key, IEnumerable<TElement> elements)
        {
            _key = key;
            _elements = elements ?? new TElement[0];
        }

        public TKey Key => _key;

        public IEnumerator<TElement> GetEnumerator() => _elements.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
