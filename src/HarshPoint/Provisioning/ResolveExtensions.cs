using HarshPoint.Provisioning.Implementation;

namespace HarshPoint.Provisioning
{
    public static class ResolveExtensions
    {
        public static IResolveBuilder<T, ClientObjectResolveContext> ValidateIsClientObjectResolveBuilder<T>(this IResolve<T> resolve)
        {
            if (resolve == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(resolve));
            }

            var result = (resolve as IResolveBuilder<T, ClientObjectResolveContext>);

            if (result == null)
            {
                throw Logger.Fatal.ArgumentNotAssignableTo(
                    nameof(resolve),
                    resolve,
                    typeof(IResolveBuilder<T, ClientObjectResolveContext>)
                );
            }

            return result;
        }

        public static IResolveBuilder<T, ClientObjectResolveContext> ValidateIsClientObjectResolveBuilder<T>(this IResolveSingle<T> resolve)
        {
            if (resolve == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(resolve));
            }

            var result = (resolve as IResolveBuilder<T, ClientObjectResolveContext>);

            if (result == null)
            {
                throw Logger.Fatal.ArgumentNotAssignableTo(
                    nameof(resolve),
                    resolve,
                    typeof(IResolveBuilder<T, ClientObjectResolveContext>)
                );
            }

            return result;
        }
        public static IResolveBuilder<T, ClientObjectResolveContext> ValidateIsClientObjectResolveBuilder<T>(this IResolveSingleOrDefault<T> resolve)
        {
            if (resolve == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(resolve));
            }

            var result = (resolve as IResolveBuilder<T, ClientObjectResolveContext>);

            if (result == null)
            {
                throw Logger.Fatal.ArgumentNotAssignableTo(
                    nameof(resolve),
                    resolve,
                    typeof(IResolveBuilder<T, ClientObjectResolveContext>)
                );
            }

            return result;
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ResolveExtensions));
    }
}
