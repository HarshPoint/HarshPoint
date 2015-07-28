using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveListContentType :
        NestedResolveBuilder<ContentType, List, ClientObjectResolveContext>
    {
        public ResolveListContentType(IResolveBuilder<List> parent)
            : base(parent)
        {
        }

        protected override void InitializeContextBeforeParent(ClientObjectResolveContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            context.Include<List>(
                list => list.ContentTypes
            );

            base.InitializeContextBeforeParent(context);
        }

        protected override IEnumerable<ContentType> SelectChildren(List parent)
        {
            if (parent == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(parent));
            }

            return parent.ContentTypes;
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolveListContentType>();
    }
}
