﻿using HarshPoint.Provisioning.Implementation;
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

        public static ResolveListById ById(this IResolveBuilder<List> parent, params Guid[] ids)
            => new ResolveListById(parent, ids);

        public static ResolveTermGroupById ById(this IResolveBuilder<TermGroup> parent, params Guid[] ids)
            => new ResolveTermGroupById(parent, ids);

        public static ResolveTermSetById ById(this IResolveBuilder<TermSet> parent, params Guid[] ids)
            => new ResolveTermSetById(parent, ids);

        public static ResolveFieldByInternalName ByInternalName(this IResolveBuilder<Field> parent, params String[] internalNames)
            => new ResolveFieldByInternalName(parent, internalNames);

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

        public static ResolveListContentType ContentType(this IResolveBuilder<List, ClientObjectResolveContext> list)
            => new ResolveListContentType(list);

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

        public static ResolveTermStoreTermGroup TermGroup(this IResolveBuilder<TermStore, ClientObjectResolveContext> termStore)
            => new ResolveTermStoreTermGroup(termStore);

        public static ResolveTermStoreTermSet TermSet(this IResolveBuilder<TermStore, ClientObjectResolveContext> termStore)
            => new ResolveTermStoreTermSet(termStore);

        public static ResolveTermStoreKeywordsDefault TermStoreKeywordsDefault()
            => new ResolveTermStoreKeywordsDefault();

        public static ResolveTermStoreSiteCollectionDefault TermStoreSiteCollectionDefault()
            => new ResolveTermStoreSiteCollectionDefault();

        public static ResolveSiteGroup SiteGroup()
            => new ResolveSiteGroup();

        public static ResolveSiteGroupById ById(this IResolveBuilder<Group> parent, params Int32[] ids)
            => new ResolveSiteGroupById(parent, ids);

        public static ResolveSiteGroupByName ByName(this IResolveBuilder<Group> parent, params String[] names)
            => new ResolveSiteGroupByName(parent, names);
    }
}
