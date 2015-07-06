using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace HarshPoint.Provisioning.Implementation
{
    [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public abstract class NestedResolvable<T1, T2, TIdentifier, TContext, TSelf> :
        NestedResolvable<T1, T2, TContext, TSelf>,
        IResolvableIdentifiers<TIdentifier>
         where TContext : HarshProvisionerContextBase
         where TSelf : NestedResolvable<T1, T2, TIdentifier, TContext, TSelf>
    {
        protected NestedResolvable(IResolve<T1> parent, IEnumerable<TIdentifier> identifiers)
            : base(parent)
        {
            if (identifiers == null)
            {
                throw Error.ArgumentNull(nameof(identifiers));
            }

            Identifiers = identifiers.ToImmutableArray();
        }

        public IImmutableList<TIdentifier> Identifiers
        {
            get;
            private set;
        }
    }
}