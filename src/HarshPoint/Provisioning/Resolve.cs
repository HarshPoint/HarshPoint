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

        public static ResolveListById ById(this IResolveBuilder<List> parent, params Guid[] ids)
            => new ResolveListById(parent, ids);


        public static ResolveTermSetById ById(this IResolveBuilder<TermSet> parent, params Guid[] ids)
            => new ResolveTermSetById(parent, ids);

        public static ResolveFieldByTitleOrInternalName ByInternalName(this IResolveBuilder<Field> parent, params String[] internalNames)
            => new ResolveFieldByTitleOrInternalName(parent, internalNames);

        public static ResolveListViewByTitle ByTitle(this IResolveBuilder<View> parent, params String[] titles)
            => new ResolveListViewByTitle(parent, titles);

        public static ResolveListByUrl ByUrl(this IResolveBuilder<List> parent, params String[] urls)
            => new ResolveListByUrl(parent, urls);

        public static ResolveListViewByUrl ByUrl(this IResolveBuilder<View> parent, params String[] urls)
            => new ResolveListViewByUrl(parent, urls);

        public static ResolveCatalog Catalog(params ListTemplateType[] templateTypes)
            => new ResolveCatalog(templateTypes);

        public static ResolveContentType ContentType() => _contentType;

        public static ResolveListContentType ContentType(this IResolveBuilder<List, ClientObjectResolveContext> list)
            => new ResolveListContentType(list);

        public static ResolveFieldById FieldById(params Guid[] ids)
            => new ResolveFieldById(ids);

        public static ResolveFieldByTitleOrInternalName FieldByInternalName(
            params String[] internalNames
        )
            => new ResolveFieldByTitleOrInternalName(internalNames);

        public static ResolveListField Field(this IResolveBuilder<List, ClientObjectResolveContext> list)
            => new ResolveListField(list);

        public static ResolveList List() => _list;

        public static ResolveListRootFolder RootFolder(this IResolveBuilder<List, ClientObjectResolveContext> list)
            => new ResolveListRootFolder(list);

        public static ResolveListView View(this IResolveBuilder<List, ClientObjectResolveContext> list)
            => new ResolveListView(list);

        public static ResolveTermStoreTermSet TermSet(this IResolveBuilder<TermStore, ClientObjectResolveContext> termStore)
            => new ResolveTermStoreTermSet(termStore);

        public static ResolveTermStoreKeywordsDefault TermStoreKeywordsDefault()
            => _termStoreKeywords;

        public static ResolveTermStoreSiteCollectionDefault TermStoreSiteCollectionDefault()
            => _termStoreSite;

        private static readonly ResolveContentType _contentType
            = new ResolveContentType();

        private static readonly ResolveList _list
            = new ResolveList();

        private static readonly ResolveTermStoreKeywordsDefault _termStoreKeywords
            = new ResolveTermStoreKeywordsDefault();

        private static readonly ResolveTermStoreSiteCollectionDefault _termStoreSite
            = new ResolveTermStoreSiteCollectionDefault();
    }
}
