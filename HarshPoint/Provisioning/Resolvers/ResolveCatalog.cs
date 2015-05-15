using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveCatalog : ResolveList<ListTemplateType, ResolveCatalog>
    {
        public ResolveCatalog(IEnumerable<ListTemplateType> identifiers)
            : base(identifiers)
        {
        }

        protected override Task<IEnumerable<List>> ResolveChainElement(HarshProvisionerContext context)
        {
            return Task.FromResult(
                Identifiers.Cast<Int32>().Select(id => context.Web.GetCatalog(id))
            );
        }
    }
}
