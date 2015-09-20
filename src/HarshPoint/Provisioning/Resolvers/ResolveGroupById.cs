using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveGroupById : ClientObjectIdentifierResolveBuilder<Group, Int32>
    {
        public ResolveGroupById(
            IResolveBuilder<Group> parent,
            IEnumerable<Int32> identifiers
        )
            : base(parent, identifiers, l => l.Id)
        {
        }
    }
}
