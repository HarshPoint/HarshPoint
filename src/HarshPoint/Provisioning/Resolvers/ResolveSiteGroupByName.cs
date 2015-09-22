using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveSiteGroupByName : ClientObjectIdentifierResolveBuilder<Group, String>
    {
        public ResolveSiteGroupByName(
            IResolveBuilder<Group> parent,
            IEnumerable<String> names
        )
            : base(parent, names, g => g.Title)
        {
        }
    }
}
