using HarshPoint.Provisioning.Resolvers;
using Microsoft.SharePoint.Client;

namespace HarshPoint.Provisioning
{
    public static class Resolve
    {
        public static ResolveCatalog Catalog(params ListTemplateType[] templateTypes)
        {
            return new ResolveCatalog(templateTypes);
        }
    }
}
