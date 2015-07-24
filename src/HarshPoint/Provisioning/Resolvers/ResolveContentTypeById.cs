using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveContentTypeById :
        IdentifierResolveBuilder<ContentType, ClientObjectResolveContext, HarshContentTypeId>
    {
        public ResolveContentTypeById(
            IResolveBuilder<ContentType, ClientObjectResolveContext> parent,
            IEnumerable<HarshContentTypeId> ids
        )
            : base(parent, ids)
        {
        }

        protected override void InitializeContextBeforeParent(ClientObjectResolveContext context)
        {
            context.Include<ContentType>(
                ct => ct.StringId
            );
        }

        protected override HarshContentTypeId GetIdentifier(ContentType result)
            => HarshContentTypeId.Parse(result.StringId);
    }
}
