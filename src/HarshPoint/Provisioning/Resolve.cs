using HarshPoint.Provisioning.Implementation;
using HarshPoint.Provisioning.Resolvers;
using Microsoft.SharePoint.Client;
using System;

namespace HarshPoint.Provisioning
{
    public static class Resolve
    {
        public static ResolveFieldById ById(this IResolveBuilder<Field, ClientObjectResolveContext> parent, params Guid[] ids)
            => new ResolveFieldById(parent, ids);

        public static ResolveListByUrl ByUrl(this IResolveBuilder<List, ClientObjectResolveContext> parent, params String[] urls)
            => new ResolveListByUrl(parent, urls);

        public static ResolveCatalog Catalog(params ListTemplateType[] templateTypes)
            => new ResolveCatalog(templateTypes);

        public static ResolveContentType ContentType()
            => new ResolveContentType();

        public static ResolveField Field()
            => new ResolveField();

        public static ResolveListField Field(this IResolveBuilder<List, ClientObjectResolveContext> list)
            => new ResolveListField(list);

        public static ResolveList List()
            => new ResolveList();

        public static ResolveListRootFolder RootFolder(this IResolveBuilder<List, ClientObjectResolveContext> list)
            => new ResolveListRootFolder(list);

        [Obsolete]
        public static OldResolveListByUrl ListByUrlOld(params String[] urls)
        {
            if (urls == null)
            {
                throw Error.ArgumentNull(nameof(urls));
            }

            return new OldResolveListByUrl(urls);
        }

        public static ResolveTermStoreKeywordsDefault TermStoreKeywordsDefault()
        {
            return new ResolveTermStoreKeywordsDefault();
        }

        public static ResolveTermStoreSiteCollectionDefault TermStoreSiteCollectionDefault()
        {
            return new ResolveTermStoreSiteCollectionDefault();
        }

        public static ResolvedResolver<T> Value<T>(params T[] values)
        {
            return new ResolvedResolver<T>(values);
        }
    }
}
