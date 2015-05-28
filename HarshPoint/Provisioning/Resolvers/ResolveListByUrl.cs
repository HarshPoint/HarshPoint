using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveListByUrl : ResolveList<String, ResolveListByUrl>
    {
        public ResolveListByUrl(IEnumerable<String> identifiers)
            : base(identifiers)
        {
        }

        protected override Task<IEnumerable<List>> ResolveChainElement(HarshProvisionerContext context)
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            return Identifiers.SelectSequentially(
                id => context.Web.GetListByWebRelativeUrl(id)
            );
        }
    }
}
