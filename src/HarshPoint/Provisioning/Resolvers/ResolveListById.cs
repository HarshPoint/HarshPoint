using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveListById : ClientObjectIdentifierResolveBuilder_OLD<List, Guid>
    {
        public ResolveListById(
            IResolveBuilder<List> parent,
            IEnumerable<Guid> identifiers
        )
            : base(parent, identifiers, l => l.Id)
        {
        }
    }
}
