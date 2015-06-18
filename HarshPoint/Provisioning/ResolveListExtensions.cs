using HarshPoint.Provisioning.Resolvers;
using Microsoft.SharePoint.Client;
using System;

namespace HarshPoint.Provisioning
{
    public static class ResolveListExtensions
    {
        public static ResolveListFieldById FieldById(this IResolve<List> list, params Guid[] ids)
        {
            return new ResolveListFieldById(list, ids);
        }

        public static ResolveListFieldByInternalName FieldByInternalName(this IResolve<List> list, params String[] names)
        {
            return new ResolveListFieldByInternalName(list, names);
        }

        public static ResolveListRootFolder RootFolder(this IResolve<List> list)
        {
            return new ResolveListRootFolder(list);
        }
    }
}
