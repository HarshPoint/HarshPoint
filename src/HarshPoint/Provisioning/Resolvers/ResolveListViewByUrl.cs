using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveListViewByUrl : IdentifierResolveBuilder<View, ClientObjectResolveContext, String>
    {
        public ResolveListViewByUrl(
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

            context.Include<List>(
                l=>l.RootFolder.ServerRelativeUrl
            );

            context.Include<View>(
                v => v.ServerRelativeUrl
            );

            base.InitializeContextBeforeParent(context);
        }

        protected override String GetIdentifierFromNested(NestedResolveResult nested)
        {
            if (nested == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(nested));
            }

            var tuple = nested.ExtractComponents<List, View>();

            return HarshUrl.GetRelativeTo(
                tuple.Item2.ServerRelativeUrl,
                tuple.Item1.RootFolder.ServerRelativeUrl
            );
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolveListViewByUrl>();
    }
}
