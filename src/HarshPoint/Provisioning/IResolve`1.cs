using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    public interface IResolve<out T> : IEnumerable<T>, IResolveBuilder
    {
    }

    [Obsolete]
    public interface IResolveOld<T>
    {
        Task<IEnumerable<T>> TryResolveAsync(IResolveContext context);
    }

    [Obsolete]
    public static class ResolveOldExtensions
    {
        public static async Task<T> TryResolveSingleAsync<T>(this IResolveOld<T> resolvable, IResolveContext context)
        {
            if (resolvable == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(resolvable));
            }

            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            var results = await resolvable.TryResolveAsync(context);
            return results.FirstOrDefault();
        }

        public static async Task<IEnumerable<T>> ResolveAsync<T>(this IResolveOld<T> resolvable, IResolveContext context)
        {
            if (resolvable == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(resolvable));
            }

            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            var results = await resolvable.TryResolveAsync(context);

            context.ValidateNoFailures();
            return results;
        }

        public static async Task<T> ResolveSingleAsync<T>(this IResolveOld<T> resolvable, IResolveContext context)
        {
            if (resolvable == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(resolvable));
            }

            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            var results = await resolvable.TryResolveAsync(context);
            context.ValidateNoFailures();

            switch (results.Count())
            {
                case 1: return results.First();
                case 0: throw Logger.Fatal.InvalidOperationFormat(SR.Resolvable_NoResult, resolvable);
                default: throw Logger.Fatal.InvalidOperationFormat(SR.Resolvable_ManyResults, resolvable);
            }
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ResolveOldExtensions));
    }
}
