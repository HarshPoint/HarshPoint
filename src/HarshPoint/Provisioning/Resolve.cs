using HarshPoint.Provisioning.Implementation;
using HarshPoint.Provisioning.Resolvers;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using System;

namespace HarshPoint.Provisioning
{
    public static class Resolve
    {
        public static ResolveContentTypeById ById(this IResolveBuilder<ContentType> parent, params HarshContentTypeId[] ids)
            => new ResolveContentTypeById(parent, ids);

        public static ResolveFieldById ById(this IResolveBuilder<Field> parent, params Guid[] ids)
            => new ResolveFieldById(parent, ids);

        public static ResolveTermSetById ById(this IResolveBuilder<TermSet> parent, params Guid[] ids)
            => new ResolveTermSetById(parent, ids);

        public static ResolveListViewByTitle ByTitle(this IResolveBuilder<View> parent, params String[] titles)
            => new ResolveListViewByTitle(parent, titles);

        public static ResolveListByUrl ByUrl(this IResolveBuilder<List> parent, params String[] urls)
            => new ResolveListByUrl(parent, urls);

        public static ResolveListViewByUrl ByUrl(this IResolveBuilder<View> parent, params String[] urls)
            => new ResolveListViewByUrl(parent, urls);

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

        public static ResolveListView View(this IResolveBuilder<List, ClientObjectResolveContext> list)
            => new ResolveListView(list);

        public static ResolveTermStoreTermSet TermSet(this IResolveBuilder<TermStore, ClientObjectResolveContext> termStore)
            => new ResolveTermStoreTermSet(termStore);

        public static ResolveTermStoreKeywordsDefault TermStoreKeywordsDefault()
            => new ResolveTermStoreKeywordsDefault();

        public static ResolveTermStoreSiteCollectionDefault TermStoreSiteCollectionDefault()
            => new ResolveTermStoreSiteCollectionDefault();

        public static ResolvedResolver<T> Value<T>(params T[] values)
        {
            return new ResolvedResolver<T>(values);
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(Resolve));
    }
}
