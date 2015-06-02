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
            var contentTypes = context.ClientContext.LoadQuery(context.Web.AvailableContentTypes);
            await context.ClientContext.ExecuteQueryAsync();

            return contentTypes.Where(
                ct => Identifiers.Contains(HarshContentTypeId.Parse(ct.StringId))
            );
        }
    }
}
