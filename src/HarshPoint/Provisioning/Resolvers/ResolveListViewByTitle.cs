using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveListViewByTitle : ClientObjectIdentifierResolveBuilder<View, String>
    {
        public ResolveListViewByTitle(
            IResolveBuilder<View> parent, 
            IEnumerable<String> identifiers
        )
            : base(
                parent, 
                identifiers, 
                StringComparer.OrdinalIgnoreCase,
                v => v.Title
            )
        {
        }
    }
}
