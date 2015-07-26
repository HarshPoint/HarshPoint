using System;
using System.Collections.Generic;
using System.Linq;

namespace HarshPoint
{
    public static class HarshGrouping
    {
        public static IGrouping<TKey, TElement> Create<TKey, TElement>(TKey key, IEnumerable<TElement> elements)
            => new HarshGrouping<TKey, TElement>(key, elements);

        public static IGrouping<TKey, TElement> Create<TKey, TElement>(TKey key, params TElement[] elements)
            => new HarshGrouping<TKey, TElement>(key, elements);

        public static Boolean IsGroupingType(Type type)
        {
            if (type == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(type));
            }

            if (type.IsConstructedGenericType)
            {
                return type.GetGenericTypeDefinition() == typeof(IGrouping<,>);
            }

            return false;
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(HarshGrouping));
    }

}
