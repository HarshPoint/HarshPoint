using HarshPoint.Provisioning.Implementation;

namespace HarshPoint.Provisioning
{
    public static class ResolveExtensions
    {
        public static IResolveBuilder<T, ClientObjectResolveContext> AsClientObjectResolveBuilder<T>(this IResolve<T> resolve)
        {
            if (resolve == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(resolve));
            }

            return (IResolveBuilder<T, ClientObjectResolveContext>)resolve;
        }

        public static IResolveBuilder<T, ClientObjectResolveContext> AsClientObjectResolveBuilder<T>(this IResolveSingle<T> resolve)
        {
            if (resolve == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(resolve));
            }

            return (IResolveBuilder<T, ClientObjectResolveContext>)resolve;
        }
        public static IResolveBuilder<T, ClientObjectResolveContext> AsClientObjectResolveBuilder<T>(this IResolveSingleOrDefault<T> resolve)
        {
            if (resolve == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(resolve));
            }

            return (IResolveBuilder<T, ClientObjectResolveContext>)resolve;
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ResolveExtensions));
    }
}
