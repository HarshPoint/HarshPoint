using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Implementation
{
    internal class ClientObjectQueryResolveBuilder<TResult, TQueryResult, TIdentifier, TSelf> :
        ClientObjectResolveBuilder<TResult, TQueryResult, TIdentifier, TSelf>
        where TResult : ClientObject
        where TQueryResult : ClientObject
        where TSelf : ClientObjectQueryResolveBuilder<TResult, TQueryResult, TIdentifier, TSelf>
    {
        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ClientObjectQueryResolveBuilder<,,,>));

        protected ClientObjectQueryResolveBuilder(
            ClientObjectResolveQuery<TResult, TQueryResult, HarshProvisionerContext, TIdentifier> query,
            IEnumerable<TIdentifier> identifiers
        ) : base(identifiers)
        {
            if (query == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(query));
            }

            Query = query;
        }

        public ClientObjectResolveQuery<TResult, TQueryResult, HarshProvisionerContext, TIdentifier> Query
        {
            get;
            private set;
        }

        protected override IQueryable<TQueryResult> CreateQuery(ResolveContext<HarshProvisionerContext> context)
        {
            return Query.CreateQuery(context);
        }

        protected override IEnumerable<TResult> TransformQueryResults(IEnumerable<TQueryResult> queryResults, ResolveContext<HarshProvisionerContext> context)
        {
            throw new NotImplementedException();
        }
    }
}
