using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace HarshPoint.Provisioning.Implementation
{
    [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public abstract class NestedResolvable<T1, T2, TIdentifier, TContext, TSelf> : NestedResolvable<T1, T2, TContext, TSelf>
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

            Identifiers = new List<TIdentifier>(identifiers);
        }

        public ICollection<TIdentifier> Identifiers
        {
            get;
            private set;
        }

        public override ResolvableChain Clone()
        {
            var result = (NestedResolvable<T1, T2, TIdentifier, TContext, TSelf>)base.Clone();
            result.Identifiers = new List<TIdentifier>(Identifiers);
            return result;
        }

        protected override Object ToLogObject()
        {
            return Identifiers;
        }
    }
}