using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
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

        protected override IEnumerable<List> ToEnumerable(Object state, ClientObjectResolveContext context)
        {
            return (IEnumerable<List>)(state);
        }
    }
}
