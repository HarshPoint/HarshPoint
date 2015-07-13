using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HarshPoint.Provisioning.Implementation
{
    public abstract class ClientObjectResolveBuilder<TResult, TQueryResult, TIdentifier, TSelf> :
        ClientObjectResolveBuilder<TResult, TQueryResult, TSelf>
        where TResult : ClientObject
        where TQueryResult : ClientObject
        where TSelf : ClientObjectResolveBuilder<TResult, TQueryResult, TIdentifier, TSelf>
    {
        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ClientObjectResolveBuilder<,,,>));

        protected ClientObjectResolveBuilder(IEnumerable<TIdentifier> identifiers)
        {
            if (identifiers == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(identifiers));
            }

            Identifiers = new Collection<TIdentifier>(
                identifiers.ToList()
            );
        }

        public Collection<TIdentifier> Identifiers { get; private set; }

        protected IEnumerable<TResult> ResolveIdentifiers(
            IEnumerable<TResult> items,
            ResolveContext<HarshProvisionerContext> context,
            Func<TResult, TIdentifier> identifierSelector,
            IEqualityComparer<TIdentifier> identifierComparer
        )
        {
            var byId = items.ToImmutableDictionaryFirstWins(
                item => identifierSelector(item),
                item => item,
                identifierComparer
            );

            foreach (var id in Identifiers)
            {
                TResult value;

                if (byId.TryGetValue(id, out value))
                {
                    yield return value;
                }
                else
                {
                    context.AddFailure(this, id);
                }
            }
        }
    }
}
