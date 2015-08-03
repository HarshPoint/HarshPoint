using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HarshPoint
{
    internal sealed class HarshRecursiveEqualityComparer : IEqualityComparer<Object>
    {
        private readonly Dictionary<TypeInfo, List<Func<Object, Object>>> _properties
            = new Dictionary<TypeInfo, List<Func<Object, Object>>>();

        private readonly Dictionary<TypeInfo, IEqualityComparer> _comparers
            = new Dictionary<TypeInfo, IEqualityComparer>();

        public HarshRecursiveEqualityComparer()
        {
            AddComparer<String>(StringComparer.Ordinal);
        }

        public void AddComparer<T>(IEqualityComparer<T> comparer)
        {
            if (comparer == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(comparer));
            }

            var nonGeneric = (comparer as IEqualityComparer);

            if (nonGeneric == null)
            {
                nonGeneric = new GenericComparerWrapper<T>(comparer);
            }

            AddComparer(typeof(T).GetTypeInfo(), nonGeneric);
        }

        public void AddComparer(TypeInfo typeInfo, IEqualityComparer comparer)
        {
            if (typeInfo == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(typeInfo));
            }

            if (comparer == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(comparer));
            }

            _comparers[typeInfo] = comparer;
        }

        public void AddProperty<T>(Func<T, Object> property)
        {
            if (property == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(property));
            }

            var ti = typeof(T).GetTypeInfo();
            var list = default(List<Func<Object, Object>>);

            if (!_properties.TryGetValue(ti, out list))
            {
                list = _properties[ti] = new List<Func<Object, Object>>();
            }

            list.Add(x => property((T)x));
        }

        public new Boolean Equals(Object x, Object y)
        {
            var xs = GetPropertyValues(x).ToArray();
            var ys = GetPropertyValues(y).ToArray();

            if (xs.Length != ys.Length)
            {
                return false;
            }

            return !xs.Zip(ys, Tuple.Create).Any(xy =>
            {
                if (ReferenceEquals(xy.Item1, xy.Item2))
                {
                    return false;
                }

                var comparer = GetComparer(xy.Item1 ?? xy.Item2);

                if (comparer != null)
                {
                    return !comparer.Equals(xy.Item1, xy.Item2);
                }

                return !Object.Equals(xy.Item1, xy.Item2);
            });
        }

        public Int32 GetHashCode(Object obj)
        {
            if (obj == null) return 0;

            unchecked
            {
                return GetPropertyValues(obj)
                    .Select(x => GetComparer(x)?.GetHashCode(x) ?? x?.GetHashCode() ?? 0)
                    .Aggregate(
                    17,
                    (acc, hash) => (acc * 486187739) + hash
                );
            }
        }

        private IEnumerable<Object> GetPropertyValues(Object value)
        {
            var stack = new Stack<Object>(new[] { value });

            while (stack.Any())
            {
                var current = stack.Pop();
                var props = GetProperties(current);
                var enumerable = current as IEnumerable;

                if (props?.Any() ?? false)
                {
                    foreach (var p in props)
                    {
                        stack.Push(p(current));
                    }
                }
                else if ((enumerable != null) && !(current is String))
                {
                    foreach (var item in enumerable)
                    {
                        stack.Push(item);
                    }
                }
                else
                {
                    yield return current;
                }
            }
        }

        private IEqualityComparer GetComparer(Object obj)
            => FindEntries(_comparers, obj).FirstOrDefault();

        private IEnumerable<Func<Object, Object>> GetProperties(Object obj)
            => FindEntries(_properties, obj).SelectMany(fns => fns);

        private static IEnumerable<T> FindEntries<T>(Dictionary<TypeInfo, T> dict, Object obj)
        {
            if (obj == null)
            {
                return new T[0];
            }

            var ti = obj.GetType().GetTypeInfo();

            return dict
                .Where(t => t.Key.IsAssignableFrom(ti))
                .Select(t => t.Value);
        }

        private sealed class GenericComparerWrapper<T> : IEqualityComparer
        {
            private readonly IEqualityComparer<T> _inner;

            public GenericComparerWrapper(IEqualityComparer<T> inner)
            {
                _inner = inner;
            }

            public new Boolean Equals(Object x, Object y)
                => _inner.Equals((T)x, (T)y);

            public Int32 GetHashCode(Object obj)
                => _inner.GetHashCode((T)obj);
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext<HarshRecursiveEqualityComparer>();
    }
}
