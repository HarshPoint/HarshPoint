using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveFieldById : ClientObjectIdentifierResolveBuilder<Field, Guid>
    {
        public ResolveFieldById(
            IResolveBuilder<Field> parent,
            IEnumerable<Guid> identifiers
        )
            : base(parent, identifiers, f => f.Id)
        {
        }
    }
}