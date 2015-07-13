using Microsoft.SharePoint.Client;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;
using System.Collections.Immutable;

namespace HarshPoint.Provisioning.Implementation
{
    public abstract class ClientObjectResolveBuilder<T, TSelf> :
        Chain<IClientObjectResolveBuilderElement<T>>,
        IClientObjectResolveBuilderElement<T>,
        IClientObjectResolveBuilder<T>
        where T : ClientObject
        where TSelf : ClientObjectResolveBuilder<T, TSelf>
    {
        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ClientObjectResolveBuilder<,>));

        private ImmutableList<Expression<Func<T, Object>>> _retrievals =
            ImmutableList<Expression<Func<T, Object>>>.Empty;

        void IClientObjectResolveBuilder<T>.Include(params Expression<Func<T, Object>>[] retrievals)
        {
            foreach (var e in Elements)
            {
                e.Include(retrievals);
            }
        }

        IEnumerable IResolveBuilder<HarshProvisionerContext>.ToEnumerable(HarshProvisionerContext context)
        {
            return Elements.SelectMany(e => e.ToEnumerable(context));
        }

        public TSelf And(Chain<IClientObjectResolveBuilderElement<T>> other)
        {
            Append(other);
            return (TSelf)(this);
        }

        public void Include(params Expression<Func<T, Object>>[] retrievals)
        {
            if (retrievals == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(retrievals));
            }

            _retrievals = _retrievals.AddRange(retrievals);
        }

        public abstract IEnumerable<T> ToEnumerable(HarshProvisionerContext context);

        protected IReadOnlyCollection<Expression<Func<T, Object>>> Retrievals
            => _retrievals;
    }
}
