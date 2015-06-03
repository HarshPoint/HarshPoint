using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveContentTypeById :
        Implementation.Resolvable<ContentType, HarshContentTypeId, HarshProvisionerContext, ResolveContentTypeById>
    {
        public ResolveContentTypeById(IEnumerable<HarshContentTypeId> identifiers)
            : base(identifiers)
        {
        }
        
        protected override async Task<IEnumerable<ContentType>> ResolveChainElement(HarshProvisionerContext context)
        {
            var results = Identifiers
                .Select(id => context.Web.ContentTypes.GetById(id.ToString()))
                .ToArray();

            foreach (var ct in results)
            {
                context.ClientContext.Load(ct);
            }

            await context.ClientContext.ExecuteQueryAsync();
            return results;
        }
    }
}
