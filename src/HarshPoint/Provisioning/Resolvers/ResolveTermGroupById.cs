using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveTermGroupById :
        ClientObjectIdentifierResolveBuilder<TermGroup, Guid>
    {
        public ResolveTermGroupById(
            IResolveBuilder<TermGroup> parent,
            IEnumerable<Guid> identifiers
        )
            : base(parent, identifiers, tg => tg.Id)
        {
        }
    }
}
