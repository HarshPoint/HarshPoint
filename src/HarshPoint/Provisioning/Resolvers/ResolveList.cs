using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Linq;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveList : ClientObjectResolveBuilder<List>
    {
        protected override IQueryable<List> CreateQuery(ClientObjectResolveContext context)
            => context.ProvisionerContext.Web.Lists;
    }
}
