using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveFieldByInternalName : ClientObjectIdentifierResolveBuilder<Field, String>
    {
        public ResolveFieldByInternalName(
            IResolveBuilder<Field> parent,
            IEnumerable<String> identifiers
        )
            : base(parent, identifiers, f => f.InternalName)
        {
        }
    }
}