using Microsoft.SharePoint.Client;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ClientObjectResolveContext : ResolveContext<HarshProvisionerContext>
    {
        private readonly IImmutableDictionary<Type, IImmutableList<Expression>> _retrievals;

        internal ClientObjectResolveContext(IImmutableDictionary<Type, IImmutableList<Expression>> retrievals = null)
        {
            _retrievals = retrievals ?? ImmutableDictionary<Type, IImmutableList<Expression>>.Empty;
        }

        public ClientObjectResolveContext Include<T>(params Expression<Func<T, Object>>[] retrievals)
            where T : ClientObject
        {
            if (retrievals == null)
            {
                throw Error.ArgumentNull(nameof(retrievals));
            }

            return new ClientObjectResolveContext(
                _retrievals.SetItem(
                    typeof(T),
                    GetRetrievals(typeof(T)).AddRange(retrievals)
                )
            );
        }

        public Expression<Func<T, Object>>[] GetRetrievals<T>()
            where T : ClientObject
        {
            return GetRetrievals(typeof(T))
                .Cast<Expression<Func<T, Object>>>()
                .ToArray();
        }

        public IImmutableList<Expression> GetRetrievals(Type type)
        {
            if (type == null)
            {
                throw Error.ArgumentNull(nameof(type));
            }

            return _retrievals.GetValueOrDefault(type, EmptyExpressionList);
        }
        
        private static readonly IImmutableList<Expression> EmptyExpressionList =
            ImmutableList<Expression>.Empty;
    }
}
