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
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            context.Include<View>(
                v => v.Title
            );

            base.InitializeContextBeforeParent(context);
        }

        protected override String GetIdentifier(View result)
        {
            if (result == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(result));
            }

            return result.Title;
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolveListViewByTitle>();
    }
}
