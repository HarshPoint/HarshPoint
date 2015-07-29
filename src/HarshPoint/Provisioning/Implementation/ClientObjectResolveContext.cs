using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HarshPoint.Provisioning.Implementation
{
    public sealed class ClientObjectResolveContext : ResolveContext<HarshProvisionerContext>
    {
        private readonly ClientObjectResolveQueryProcessor _queryProcessor
            = new ClientObjectResolveQueryProcessor();

        public ClientObjectResolveContext(HarshProvisionerContext provisionerContext)
            : base(provisionerContext)
        {
        }

        public void Include<T>(params Expression<Func<T, Object>>[] retrievals)
            where T : ClientObject
        {
            if (retrievals == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(retrievals));
            }

            _queryProcessor.Include(retrievals);
        }

        public IEnumerable<T> LoadQuery<T>(IQueryable<T> query)
            where T : ClientObject
        {
            if (query == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(query));
            }

            query = _queryProcessor.Process(query);

            return ProvisionerContext.ClientContext.LoadQuery(
                query
            );
        }

        public void Load<T>(T clientObject, params Expression<Func<T, Object>>[] retrievals)
            where T : ClientObject
        {
            if (clientObject == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(clientObject));
            }

            if (retrievals == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(retrievals));
            }

            if (retrievals.Any())
            {
                Include(retrievals);
            }

            Load(clientObject);
        }

        public void Load<T>(T clientObject)
            where T : ClientObject
        {
            if (clientObject == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(clientObject));
            }

            ProvisionerContext.ClientContext.Load(
                clientObject,
                _queryProcessor.GetRetrievals<T>()
            );
        }

        internal ClientObjectResolveQueryProcessor QueryProcessor => _queryProcessor;

        private static readonly HarshLogger Logger = HarshLog.ForContext<ClientObjectResolveContext>();
    }
}
