using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace HarshPoint.Provisioning.Implementation
{
    public abstract class ClientObjectIdentifierResolveBuilder<TResult, TIdentifier, TParent> :
        ClientObjectResolveBuilder<TResult>
        where TResult : ClientObject
    {
        private readonly IResolveBuilder<TParent> _parent;
        private readonly ImmutableArray<TIdentifier> _identifiers;

        protected ClientObjectIdentifierResolveBuilder(
            IResolveBuilder<TParent> parent,
            IEnumerable<TIdentifier> identifiers
        )
        {
            if (parent == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(parent));
            }

            if (identifiers == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(identifiers));
            }

            _parent = parent;
            _identifiers = identifiers.ToImmutableArray();
        }

        protected override IEnumerable<TResult> CreateObjects(ClientObjectResolveContext context)
        {
            var state = _parent.Initialize(context);
            var parents = _parent.ToEnumerable(context, state).Cast<TParent>();

            var results = new List<TResult>();

            foreach (var parent in parents)
            {
                var selectors = GetSelectors(context.ProvisionerContext)
                    .Select(sel => new Func<TIdentifier, TResult>(id => sel(parent, id)))
                    .ToArray();

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
            }
            return results;
        }

        protected abstract IEnumerable<Func<TParent, TIdentifier, TResult>> GetSelectors(
            HarshProvisionerContext context
        );

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ClientObjectIdentifierResolveBuilder<,,>));
    }
}
