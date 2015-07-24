using HarshPoint.Provisioning.Implementation;
using HarshPoint.Provisioning.Resolvers;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveFieldById : IdentifierResolveBuilder<Field, ClientObjectResolveContext, Guid>
    {
        public ResolveFieldById(
            IResolveBuilder<Field, ClientObjectResolveContext> parent,
            IEnumerable<Guid> identifiers
        )
            : base(parent, identifiers)
        {
        }

        protected override void InitializeContextBeforeParent(ClientObjectResolveContext context)
        {
            context.Include<Field>(
                f => f.Id
            );
        }

        protected override Guid GetIdentifier(Field result) => result.Id;
    }
}