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

        public IResolve<T> Resolve<T>(IResolve<T> value)
            => Bind(value);

        public IResolveSingle<T> ResolveSingle<T>(IResolveSingle<T> value)
            => Bind(value);

        public IResolveSingleOrDefault<T> ResolveSingleOrDefault<T>(IResolveSingleOrDefault<T> value)
            => Bind(value);

        protected T Bind<T>(T value)
            => Bind(value, null);

        protected T Bind<T>(T value , Func<IResolveContext> contextFactory)
        {
            if (value == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(value));
            }

            var binder = new ResolvedPropertyBinder(typeof(Holder<T>));
            var holder = new Holder<T>() { Value = value };

            binder.Bind(
                holder, 
                contextFactory ?? ResolveContextFactory
            );

            return holder.Value;
        }

        private Func<IResolveContext> ResolveContextFactory { get; }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ManualResolver));

        private sealed class Holder<T>
        {
            public T Value { get; set; }
        }
    }
}
