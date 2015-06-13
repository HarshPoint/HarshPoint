using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace HarshPoint.Provisioning.Implementation
{
    [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public abstract class Resolvable<T, TIdentifier, TContext, TSelf>
        : Resolvable<T, TContext, TSelf>
        where T : class
        where TContext : HarshProvisionerContextBase
        where TSelf : Resolvable<T, TIdentifier, TContext, TSelf>
    {
        protected Resolvable(IEnumerable<TIdentifier> identifiers)
            : this(identifiers, null)
        {
        }

        protected Resolvable(IEnumerable<TIdentifier> identifiers, IEqualityComparer<TIdentifier> identifierComparer)
        {
            if (identifiers == null)
            {
                throw Error.ArgumentNull(nameof(identifiers));
            }

            Identifiers = ImmutableHashSet.CreateRange(identifierComparer, identifiers);
            IdentifierComparer = identifierComparer;
        }

        public IImmutableSet<TIdentifier> Identifiers
        {
            get;
            private set;
        }

        public IEqualityComparer<TIdentifier> IdentifierComparer
        {
            get;
            private set;
        }

        protected override Object ToLogObject()
        {
            return Identifiers;
        }
    }
}
