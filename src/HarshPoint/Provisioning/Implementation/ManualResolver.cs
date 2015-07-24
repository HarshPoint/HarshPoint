using System;

namespace HarshPoint.Provisioning.Implementation
{
    public sealed class ManualResolver
    {
        public ManualResolver(Func<IResolveContext> resolveContextFactory)
        {
            if (resolveContextFactory == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(resolveContextFactory));
            }

            ResolveContextFactory = resolveContextFactory;
        }

        public IResolve<T> Resolve<T>(IResolve<T> resolve)
            => Bind(Tuple.Create(resolve)).Item1;

        public IResolveSingle<T> ResolveSingle<T>(IResolveSingle<T> resolve)
            => Bind(Tuple.Create(resolve)).Item1;

        public IResolveSingleOrDefault<T> ResolveSingleOrDefault<T>(IResolveSingleOrDefault<T> resolve)
            => Bind(Tuple.Create(resolve)).Item1;
        
        private T Bind<T>(T obj)
        {
            new ResolvedPropertyBinder(typeof(T)).Bind(obj, ResolveContextFactory);
            return obj;
        }

        private Func<IResolveContext> ResolveContextFactory { get; set; }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ManualResolver>();

    }
}
