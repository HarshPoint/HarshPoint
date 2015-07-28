using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveListViewByTitle : IdentifierResolveBuilder<View, ClientObjectResolveContext, String>
    {
        public ResolveListViewByTitle(
            IResolveBuilder<View> parent, 
            IEnumerable<String> identifiers
        )
            : base(parent, identifiers, StringComparer.OrdinalIgnoreCase)
        {
        }

        protected override void InitializeContextBeforeParent(ClientObjectResolveContext context)
        {
            context.Include<View>(
                v => v.Title
            );

            base.InitializeContextBeforeParent(context);
        }

        protected override string GetIdentifier(View result)
            => result.Title;
    }
}
