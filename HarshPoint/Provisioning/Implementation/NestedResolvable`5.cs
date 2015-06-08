using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace HarshPoint.Provisioning.Implementation
{
    [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public abstract class NestedResolvable<T1, T2, TIdentifier, TContext, TSelf> : NestedResolvable<T1, T2, TContext, TSelf>
         where TContext : HarshProvisionerContextBase
         where TSelf : NestedResolvable<T1, T2, TIdentifier, TContext, TSelf>
    {
        protected NestedResolvable(IResolve<T1> parent, IEnumerable<TIdentifier> identifiers, IEqualityComparer<TIdentifier> idComparer = null)
            : base(parent)
        {
            if (identifiers == null)
            {
                throw Error.ArgumentNull(nameof(identifiers));
            }

            Identifiers = identifiers.ToImmutableHashSet();
            IdentifierComparer = idComparer ?? EqualityComparer<TIdentifier>.Default;
        }

        public IEqualityComparer<TIdentifier> IdentifierComparer
        {
            get;
            private set;
        }

        public IImmutableSet<TIdentifier> Identifiers
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