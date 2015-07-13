using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveListByUrl :
        ClientObjectResolveBuilder<List, List, String, ResolveListByUrl>
    {
        private readonly ClientObjectResolveQuery<List, List, Web, String> Query =
            ClientObjectResolveQuery.ListByUrl;

        public ResolveListByUrl(IEnumerable<String> urls)
            : base(urls)
        {
        }

        protected override IQueryable<List> CreateQuery(ResolveContext<HarshProvisionerContext> context)
        {
            return Query.CreateQuery(context.ProvisionerContext.Web);
        }
        
        protected override IEnumerable<List> TransformQueryResults(IEnumerable<List> queryResults, ResolveContext<HarshProvisionerContext> context)
        {
            return ResolveIdentifiers(
                queryResults, 
                context,
                list => Query.IdentifierSelector(context.ProvisionerContext.Web, list), 
                Query.IdentifierComparer
            );
        }
    }
}
