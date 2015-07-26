using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarshPoint
{
    public static class HarshGrouping
    {
        public static IGrouping<TKey, TElement> Create<TKey, TElement>(TKey key, IEnumerable<TElement> elements)
            => new HarshGrouping<TKey, TElement>(key, elements);

        public static IGrouping<TKey, TElement> Create<TKey, TElement>(TKey key, params TElement[] elements)
            => new HarshGrouping<TKey, TElement>(key, elements);
    }

}
