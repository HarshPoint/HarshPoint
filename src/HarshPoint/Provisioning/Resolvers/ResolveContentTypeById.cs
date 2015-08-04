using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveContentTypeById :
        IdentifierResolveBuilder<ContentType, ClientObjectResolveContext, HarshContentTypeId>
    {
        public ResolveContentTypeById(
            IResolveBuilder<ContentType> parent,
            IEnumerable<HarshContentTypeId> ids
        )
            : base(parent, ids)
        {
        }

        protected override void InitializeContextBeforeParent(ClientObjectResolveContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            context.Include<ContentType>(
                ct => ct.StringId
            );

            base.InitializeContextBeforeParent(context);
        }

        protected override HarshContentTypeId GetIdentifier(ContentType result)
        {
            if (result == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(result));
            }

            return HarshContentTypeId.Parse(result.StringId);
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolveContentTypeById>();
    }
}
