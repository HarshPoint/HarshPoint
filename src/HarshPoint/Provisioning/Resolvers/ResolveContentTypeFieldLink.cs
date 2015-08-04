using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveContentTypeFieldLink :
        NestedResolveBuilder<FieldLink, ContentType, ClientObjectResolveContext>
    {
        public ResolveContentTypeFieldLink(IResolveBuilder<ContentType, ClientObjectResolveContext> parent) 
            : base(parent)
        {
        }

        protected override void InitializeContextBeforeParent(ClientObjectResolveContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            context.Include<ContentType>(
                ct => ct.FieldLinks
            );

            base.InitializeContextBeforeParent(context);
        }

        protected override IEnumerable<FieldLink> SelectChildren(ContentType parent)
        {
            if (parent == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(parent));
            }

            return parent.FieldLinks;
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolveContentTypeFieldLink>();
    }
}
