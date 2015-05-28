using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace HarshPoint.Provisioning.Implementation
{
    [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public abstract class Resolvable<T, TIdentifier, TContext, TSelf> 
        : Resolvable<T, TContext, TSelf>
        where TContext : HarshProvisionerContextBase
        where TSelf : Resolvable<T, TIdentifier, TContext, TSelf>
    {
        protected Resolvable(IEnumerable<TIdentifier> identifiers)
        {
            if (identifiers == null)
            {
                throw Error.ArgumentNull(nameof(identifiers));
            }

            Identifiers = ImmutableHashSet.CreateRange(identifiers);
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
