using HarshPoint.Provisioning.Implementation;
using HarshPoint.Provisioning.Resolvers;
using Microsoft.SharePoint.Client;
using System;
using System.Linq;

namespace HarshPoint.Provisioning
{
    public static class Resolve
    {
        public static ResolveCatalog Catalog(params ListTemplateType[] templateTypes)
        {
            if (templateTypes == null)
            {
                throw Error.ArgumentNull(nameof(templateTypes));
            }

            return new ResolveCatalog(templateTypes);
        }

        public static ResolveContentTypeById ContentTypeById(params String[] ids)
        {
            if (ids == null)
            {
                throw Error.ArgumentNull(nameof(ids));
            }

            return new ResolveContentTypeById(
                ids.Select(HarshContentTypeId.Parse)
            );
        }

        public static ResolveContentTypeById ContentTypeById(params HarshContentTypeId[] ids)
        {
            if (ids == null)
            {
                throw Error.ArgumentNull(nameof(ids));
            }

            return new ResolveContentTypeById(ids);
        }
        
        public static ResolveFieldById FieldById(params Guid[] ids)
        {
            if (ids == null)
            {
                throw Error.ArgumentNull(nameof(ids));
            }

            return new ResolveFieldById(ids);
        }

        public static ResolveListByUrl ListByUrl(params String[] urls)
        {
            if (urls == null)
            {
                throw Error.ArgumentNull(nameof(urls));
            }


            return new ResolveListByUrl(urls);
        }

        public static ResolvedResolver<T> Value<T>(params T[] values)
        {
            return new ResolvedResolver<T>(values);
        }
    }
}
