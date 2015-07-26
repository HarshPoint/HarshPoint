using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace HarshPoint.Provisioning
{
    [Obsolete]
    public static class ResolveClientObjectExtension
    {
        public static IResolveOld<T> Include<T>(
            this IResolveOld<T> resolvable,
            params Expression<Func<T, Object>>[] retrievals
        )
            where T : ClientObject
        => CreateModifier(resolvable, retrievals);

        public static IResolveOld<IGrouping<T1, T2>> Include<T1, T2>(
            this IResolveOld<IGrouping<T1, T2>> resolvable,
            params Expression<Func<T2, Object>>[] retrievals
        )
            where T2 : ClientObject
        => CreateModifier(resolvable, retrievals);

        public static IResolveOld<Tuple<T1, T2>> Include<T1, T2>(
            this IResolveOld<Tuple<T1, T2>> resolvable,
            params Expression<Func<T2, Object>>[] retrievals
        )
            where T2 : ClientObject
        => CreateModifier(resolvable, retrievals);

        public static IResolveOld<IGrouping<T1, T2>> IncludeOnParent<T1, T2>(
            this IResolveOld<IGrouping<T1, T2>> resolvable,
            params Expression<Func<T1, Object>>[] retrievals
        )
            where T1 : ClientObject
        => CreateModifier(resolvable, retrievals);

        public static IResolveOld<Tuple<T1, T2>> IncludeOnParent<T1, T2>(
            this IResolveOld<Tuple<T1, T2>> resolvable,
            params Expression<Func<T1, Object>>[] retrievals
        )
            where T1 : ClientObject
        => CreateModifier(resolvable, retrievals);

        private static IResolveOld<TResolved> CreateModifier<TResolved, TClientObject>(
            IResolveOld<TResolved> resolvable,
            Expression<Func<TClientObject, Object>>[] retrievals
        )
            where TClientObject : ClientObject
        {
            if (resolvable == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(resolvable));
            }

            if (retrievals == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(retrievals));
            }

            return new ResolvableContextModification<TResolved>(
                resolvable, 
                ctx => ((ClientObjectResolveContext)(ctx)).Include(retrievals)
            );
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ResolveClientObjectExtension));
    }
}
