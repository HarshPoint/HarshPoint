using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveCatalog : ClientObjectResolveBuilder<List>
    {
        public ResolveCatalog(IEnumerable<ListTemplateType> identifiers)
        {
            Identifiers = new HashSet<ListTemplateType>(identifiers);
        }

        public HashSet<ListTemplateType> Identifiers { get; private set; }

        protected override Object Initialize(ClientObjectResolveContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            var lists = Identifiers
                .Cast<Int32>()
                .Select(context.ProvisionerContext.Web.GetCatalog)
                .ToArray();

            foreach (var list in lists)
            {
                context.Load(list);
            }

            return lists;
        }

        protected override IEnumerable ToEnumerable(Object state, ClientObjectResolveContext context)
            => (IEnumerable)(state);

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolveCatalog>();
    }
}
