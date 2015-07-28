using Microsoft.SharePoint.Client;
using System;
using System.Linq.Expressions;

namespace HarshPoint.Provisioning.Implementation
{
    public sealed class ClientObjectManualResolver : ManualResolver
    {
        public ClientObjectManualResolver(Func<ClientObjectResolveContext> resolveContextFactory)
            : base(resolveContextFactory)
        {
            ResolveContextFactory = resolveContextFactory;
        }

        public IResolve<T> Resolve<T>(
            IResolve<T> resolve, 
            params Expression<Func<T, Object>>[] retrievals
        )
            where T : ClientObject
            => Bind(resolve, retrievals);

        public IResolveSingle<T> ResolveSingle<T>(
            IResolveSingle<T> resolve,
            params Expression<Func<T, Object>>[] retrievals
        )
            where T : ClientObject
            => Bind(resolve, retrievals);

        public IResolveSingleOrDefault<T> ResolveSingleOrDefault<T>(
            IResolveSingleOrDefault<T> resolve,
            params Expression<Func<T, Object>>[] retrievals
        )
            where T : ClientObject
            => Bind(resolve, retrievals);

        private TResolve Bind<TResolve, TResult>(TResolve obj, Expression<Func<TResult, Object>>[] retrievals)
            where TResult : ClientObject
        {
            var factory = ResolveContextFactory;

            if ((retrievals != null) && retrievals.Any())
            {
                factory = delegate
                {
                    var corc = ResolveContextFactory();
                    corc.Include(retrievals);
                    return corc;
                };
            }

            return Bind(obj, factory);
        }

        private Func<ClientObjectResolveContext> ResolveContextFactory { get; set; }
    }
}
