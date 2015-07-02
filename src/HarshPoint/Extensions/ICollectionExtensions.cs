using System.Collections.Generic;

namespace HarshPoint
{
    public static class ICollectionExtensions
    {
        public static void AddIfNotContains<T>(this ICollection<T> collection, T item)
        {
            if (collection == null)
            {
                throw Error.ArgumentNull(nameof(collection));
            }

            if (collection.Contains(item))
            {
                return;
            }

            collection.Add(item);
        }

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            if (collection == null)
            {
                throw Error.ArgumentNull(nameof(collection));
            }

            if (items == null)
            {
                throw Error.ArgumentNull(nameof(items));
            }

            var list = (collection as List<T>);
            var hashset = (collection as HashSet<T>);

            if (list != null)
            {
                list.AddRange(items);
            }
            else if (hashset != null)
            {
                hashset.UnionWith(items);
            }
            else
            {
                foreach (var item in items)
                {
                    collection.Add(item);
                }
            }
        }
    }
}
