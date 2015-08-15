using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveCatalog : ClientObjectResolveBuilder<List>
    {
        public ResolveCatalog(IEnumerable<ListTemplateType> identifiers)
        {
            if (identifiers == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(identifiers));
            }

            Identifiers = identifiers.ToImmutableHashSet();
        }

        public ImmutableHashSet<ListTemplateType> Identifiers { get; }

        protected override IEnumerable<List> CreateObjects(ClientObjectResolveContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            return Identifiers
                .Cast<Int32>()
                .Select(context.ProvisionerContext.Web.GetCatalog);
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolveCatalog>();
    }
}
