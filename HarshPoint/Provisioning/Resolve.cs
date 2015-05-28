using HarshPoint.Provisioning.Resolvers;
using Microsoft.SharePoint.Client;
using System;

namespace HarshPoint.Provisioning
{
    public static class Resolve
    {
        public static ResolveCatalog Catalog(params ListTemplateType[] templateTypes)
        {
            return new ResolveCatalog(templateTypes);
        }

        public static ResolveListByUrl ListByUrl(params String[] urls)
        {
            return new ResolveListByUrl(urls);
        }
    }
}
