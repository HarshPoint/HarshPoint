using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveTermSetById :
        ClientObjectIdentifierResolveBuilder<TermSet, Guid>
    {
        public ResolveTermSetById(
            IResolveBuilder<TermSet> parent,
            IEnumerable<Guid> identifiers
        )
            : base(parent, identifiers, ts => ts.Id)
        {
        }
    }
}
