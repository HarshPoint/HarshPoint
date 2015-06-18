using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace HarshPoint.Provisioning
{
    public static class ResolveClientObjectExtension
    {
        public static IResolve<T> Include<T>(
            this IResolve<T> resolvable,
            params Expression<Func<T, Object>>[] retrievals
        )
            where T : ClientObject
        => CreateModifier(resolvable, retrievals);

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static IResolve<IGrouping<T1, T2>> Include<T1, T2>(
            this IResolve<IGrouping<T1, T2>> resolvable,
            params Expression<Func<T2, Object>>[] retrievals
        )
            where T2 : ClientObject
        => CreateModifier(resolvable, retrievals);

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static IResolve<Tuple<T1, T2>> Include<T1, T2>(
            this IResolve<Tuple<T1, T2>> resolvable,
            params Expression<Func<T2, Object>>[] retrievals
        )
            where T2 : ClientObject
        => CreateModifier(resolvable, retrievals);

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static IResolve<IGrouping<T1, T2>> IncludeOnParent<T1, T2>(
            this IResolve<IGrouping<T1, T2>> resolvable,
            params Expression<Func<T1, Object>>[] retrievals
        )
            where T1 : ClientObject
        => CreateModifier(resolvable, retrievals);

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static IResolve<Tuple<T1, T2>> IncludeOnParent<T1, T2>(
            this IResolve<Tuple<T1, T2>> resolvable,
            params Expression<Func<T1, Object>>[] retrievals
        )
            where T1 : ClientObject
        => CreateModifier(resolvable, retrievals);

        private static IResolve<TResolved> CreateModifier<TResolved, TClientObject>(
            IResolve<TResolved> resolvable,
            Expression<Func<TClientObject, Object>>[] retrievals
        )
            where TClientObject : ClientObject
        {
            if (resolvable == null)
            {
                throw Error.ArgumentNull(nameof(resolvable));
            }

            if (retrievals == null)
            {
                throw Error.ArgumentNull(nameof(retrievals));
            }

            return new ResolvableContextModification<TResolved>(resolvable, ctx =>
            {
                var clientCtx = (ClientObjectResolveContext)(ctx);
                clientCtx.Include(retrievals);
            });
        }
    }
}
