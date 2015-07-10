using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace HarshPoint.Provisioning.Implementation
{
    public class Chain<TElement>
    {
        private static readonly HarshLogger Logger = HarshLog.ForContext<Chain<TElement>>();

        protected void Append(Chain<TElement> other)
        {
            if (other == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(other));
            }

            var thisElements = GetChainElements().ToImmutableHashSet();

            if (other.GetChainElements().Any(el => thisElements.Contains(el)))
            {
                throw Logger.Fatal.ArgumentOutOfRange(
                    nameof(other),
                    SR.Chain_ElementAlreadyContained
                );
            }

            GetChainElements().Last().Next = other;
        }

        protected IEnumerable<TElement> Elements
            => GetChainElements().Cast<TElement>();

        private Chain<TElement> Next
        {
            get;
            set;
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
    }
}
