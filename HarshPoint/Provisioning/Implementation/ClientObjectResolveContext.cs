using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ClientObjectResolveContext<T> : ResolveContext<HarshProvisionerContext>
    {
        private IImmutableList<Expression<Func<T, Object>>> _retrievals;

        public ClientObjectResolveContext(IEnumerable<Expression<Func<T, Object>>> retrievals)
        {
            if (retrievals == null)
            {
                throw Error.ArgumentNull(nameof(retrievals));
            }

            _retrievals = retrievals.ToImmutableArray();
        }

        public IReadOnlyCollection<Expression<Func<T, Object>>> Retrievals => _retrievals;
    }
}
