using System;

namespace HarshPoint.Provisioning.Implementation
{
    public class ManualResolver
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
            => Bind(resolve);

        public IResolveSingle<T> ResolveSingle<T>(IResolveSingle<T> resolve)
            => Bind(resolve);

        public IResolveSingleOrDefault<T> ResolveSingleOrDefault<T>(IResolveSingleOrDefault<T> resolve)
            => Bind(resolve);

        protected T Bind<T>(T obj)
            => Bind(obj, null);

        protected T Bind<T>(T obj, Func<IResolveContext> contextFactory)
        {
            if (obj == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(obj));
            }

            var binder = new ResolvedPropertyBinder(typeof(Holder<T>));
            var holder = new Holder<T>() { Value = obj };

            binder.Bind(
                holder, 
                contextFactory ?? ResolveContextFactory
            );

            return holder.Value;
        }

        private Func<IResolveContext> ResolveContextFactory { get; set; }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ManualResolver));

        private sealed class Holder<T>
        {
            public T Value { get; set; }
        }
    }
}
