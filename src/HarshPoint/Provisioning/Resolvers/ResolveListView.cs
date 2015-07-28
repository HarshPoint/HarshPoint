using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveListView : NestedResolveBuilder<View, List, ClientObjectResolveContext>
    {
        public ResolveListView(IResolveBuilder<List, ClientObjectResolveContext> parent)
            : base(parent)
        {
        }

        protected override void InitializeContextBeforeParent(ClientObjectResolveContext context)
        {
            context.Include<List>(
                l => l.Views
            );

            base.InitializeContextBeforeParent(context);
        }

        protected override IEnumerable<View> SelectChildren(List parent)
            => parent.Views;
    }
}
