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

        protected override Task<IEnumerable<List>> ResolveChainElement(ResolveContext<HarshProvisionerContext> context)
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            return Task.FromResult(
                Identifiers.Cast<Int32>().Select(id => context.ProvisionerContext.Web.GetCatalog(id))
            );
        }
    }
}
