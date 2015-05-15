using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace HarshPoint.Provisioning.Implementation
{
    [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public abstract class Resolvable<T, TIdentifier, TContext, TSelf> : Resolvable<T, TContext, TSelf>
        where TContext : HarshProvisionerContextBase
        where TSelf : Resolvable<T, TIdentifier, TContext, TSelf>
    {
        protected Resolvable(IEnumerable<TIdentifier> identifiers)
        {
            if (identifiers == null)
            {
                throw Error.ArgumentNull(nameof(identifiers));
            }

            Identifiers = new List<TIdentifier>(identifiers);
        }

        public ICollection<TIdentifier> Identifiers
        {
            get;
            private set;
        }

        public override ResolvableChain Clone()
        {
            var result = (Resolvable<T, TIdentifier, TContext, TSelf>)base.Clone();
            result.Identifiers = new List<TIdentifier>(Identifiers);
            return result;
        }

        protected override Object ToLogObject()
        {
            return Identifiers;
        }
    }
}
