using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace HarshPoint
{
    public struct HarshDependencyGraph<T> : IEquatable<HarshDependencyGraph<T>>
    {
        private readonly ImmutableDictionary<T, ImmutableHashSet<T>> _graph;

        public HarshDependencyGraph(IEqualityComparer<T> comparer)
        {
            _graph = ImmutableDictionary.Create<T, ImmutableHashSet<T>>(comparer);
        }

        private HarshDependencyGraph(ImmutableDictionary<T, ImmutableHashSet<T>> graph)
        {
            _graph = graph;
        }

        public HarshDependencyGraph<T> AddNode(T node)
        {
            if (Graph.ContainsKey(node))
            {
                return this;
            }

            return new HarshDependencyGraph<T>(
                Graph.Add(node, ImmutableHashSet.Create(Comparer))
            );
        }

        public HarshDependencyGraph<T> AddEdge(T from, T to)
        {
            if (Comparer.Equals(from, to))
            {
                throw Logger.Fatal.ArgumentFormat(
                    nameof(to),
                    SR.HarshDependencyGraph_EdgeFromToEqual,
                    from,
                    to
                );
            }

            var graph = AddNode(from).AddNode(to);

            return new HarshDependencyGraph<T>(
                graph.Graph.SetItem(
                    from,
                    graph.Graph[from].Add(to)
                )
            );
        }

        public override Int32 GetHashCode()
            => _graph?.GetHashCode() ?? 0;

        public override Boolean Equals(Object obj)
        {
            if (obj is HarshDependencyGraph<T>)
            {
                return Equals((HarshDependencyGraph<T>)(obj));
            }

            return base.Equals(obj);
        }

        public Boolean Equals(HarshDependencyGraph<T> other)
            => Equals(_graph, other._graph);

        public IEnumerable<T> Sort()
        {
            if (Graph.Count < 2)
            {
                return Graph.Keys;
            }

            var node = default(T);
            var results = ImmutableArray.CreateBuilder<T>(Graph.Count);
            var pending = Graph;

            while (NextRootNode(pending, out node))
            {
                pending = pending.Remove(node);

                pending = pending.Aggregate(
                    pending,
                    (acc, kvp) => acc.SetItem(kvp.Key, kvp.Value.Remove(node))
                );

                results.Add(node);
            }

            if (results.Count != Graph.Count)
            {
                throw Logger.Fatal.Write(
                    new HarshDependencyGraphCycleException()
                );
            }

            return results.ToImmutable();
        }

        private IEqualityComparer<T> Comparer
            => _graph?.KeyComparer ?? EqualityComparer<T>.Default;

        private ImmutableDictionary<T, ImmutableHashSet<T>> Graph
            => _graph ?? ImmutableDictionary<T, ImmutableHashSet<T>>.Empty;

        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static readonly HarshDependencyGraph<T> Empty
            = new HarshDependencyGraph<T>();

        public static Boolean operator ==(HarshDependencyGraph<T> left, HarshDependencyGraph<T> right)
            => left.Equals(right);

        public static Boolean operator !=(HarshDependencyGraph<T> left, HarshDependencyGraph<T> right)
            => !left.Equals(right);

        private static Boolean NextRootNode(ImmutableDictionary<T, ImmutableHashSet<T>> pending, out T result)
        {
            foreach (var item in pending)
            {
                if (!item.Value.Any())
                {
                    result = item.Key;
                    return true;
                }
            }

            result = default(T);
            return false;
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(HarshDependencyGraph<>));
    }
}
