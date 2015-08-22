using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace HarshPoint.ObjectModel
{
    public class Chain<TElement> : IHarshCloneable
    {
        protected Chain() { }

        protected Chain(Chain<TElement> next)
        {
            Next = next;
        }

        protected Chain<TElement> Append(Chain<TElement> other)
        {
            if (other == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(other));
            }

            var thisElements = GetChainElements().ToImmutableHashSet();
            var otherElements = other.GetChainElements();

            if (thisElements.Overlaps(otherElements))
            {
                throw Logger.Fatal.Argument(
                    nameof(other),
                    SR.Chain_ElementAlreadyContained
                );
            }

            var clone = Clone();
            clone.GetChainElements().Last().Next = other.Clone();
            return clone;
        }

        protected Chain<TElement> WithNext(Chain<TElement> next)
            => WithNext(next, null);

        protected Chain<TElement> WithNext(
            Chain<TElement> next,
            Action<TElement> modifier
        )
        {
            if (Next == next)
            {
                return this;
            }

            if (modifier == null)
            {
                return this.With(c => c.Next = next);
            }

            return this.With(c =>
            {
                c.Next = next;
                modifier((TElement)(Object)c);
            });
        }

        protected IEnumerable<TElement> Elements => GetChainElements().Cast<TElement>();

        protected TElement NextElement => (TElement)(Object)Next;

        private Chain<TElement> Next { get; set; }

        private Chain<TElement> Clone()
        {
            var clone = (Chain<TElement>)MemberwiseClone();
            clone.Next = Next?.Clone();
            return clone;
        }

        private IEnumerable<Chain<TElement>> GetChainElements()
        {
            var current = this;

            while (current != null)
            {
                yield return current;
                current = current.Next;
            }
        }

        Object IHarshCloneable.Clone() => Clone();

        private static readonly HarshLogger Logger
            = HarshLog.ForContext<Chain<TElement>>();
    }
}
