using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Linq;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveList : ClientObjectResolveBuilder<List>
    {
        protected override IQueryable<List> CreateQuery(ClientObjectResolveContext context)
        {
            return context.ProvisionerContext.Web.Lists;
        }

        public ResolveListField Field 
            => new ResolveListField(this);

        public ResolveListByUrl ByUrl(params String[] urls) 
            => new ResolveListByUrl(this, urls);
    }
}
