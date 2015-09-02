using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace HarshPoint.Provisioning.Implementation
{
    public abstract class ClientObjectIdentifierResolveBuilder<TResult, TIdentifier> :
        ClientObjectResolveBuilder<TResult>
        where TResult : ClientObject
    {
        private readonly ImmutableArray<TIdentifier> _identifiers;

        protected ClientObjectIdentifierResolveBuilder(
            IEnumerable<TIdentifier> identifiers
        )
        {
            if (identifiers == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(identifiers));
            }

            _identifiers = identifiers.ToImmutableArray();
        }

        protected abstract IEnumerable<Func<TIdentifier, TResult>> GetSelectors(
            HarshProvisionerContext context
        );

        protected override IEnumerable<TResult> CreateObjects(
            ClientObjectResolveContext context
        )
        {
            var selectors = GetSelectors(context.ProvisionerContext);
            var results = new List<TResult>();

            foreach (var id in _identifiers)
            {
                results.AddRange(
                    ClientObjectIdentifierResolveBuilder.ProcessSelectors(
                        this,
                        id, 
                        selectors,
                        context
                    )
                );
            }

            return results;
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ClientObjectIdentifierResolveBuilder<,>));
    }
}
