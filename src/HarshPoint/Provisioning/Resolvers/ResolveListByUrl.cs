using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Implementation
{
    public class ResolveListByUrl : IdentifierResolveBuilder<List, ClientObjectResolveContext, String>
    {
        public ResolveListByUrl(
            IResolveBuilder<List, ClientObjectResolveContext> parent,
            IEnumerable<String> identifiers
        )
            : base(parent, identifiers)
        {
        }

        protected override void InitializeContext(ClientObjectResolveContext context)
        {
            context.Include<List>(
                list => list.ParentWebUrl,
                list => list.RootFolder.ServerRelativeUrl
            );
        }

        protected override String GetIdentifier(List result)
            => HarshUrl.GetRelativeTo(result.RootFolder.ServerRelativeUrl, result.ParentWebUrl);
    }
}
