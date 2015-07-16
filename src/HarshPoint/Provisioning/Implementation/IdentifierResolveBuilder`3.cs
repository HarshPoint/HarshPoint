using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace HarshPoint.Provisioning.Implementation
{
    public abstract class IdentifierResolveBuilder<TResult, TContext, TIdentifier> :
        NestedResolveBuilder<TResult, TResult, TContext>
        where TResult : ClientObject
        where TContext : class, IResolveContext
    {
        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(IdentifierResolveBuilder<,,>));

        protected IdentifierResolveBuilder(
            IResolveBuilder<TResult, TContext> parent,
            IEnumerable<TIdentifier> identifiers
        )
            : base(parent)
        {
            if (identifiers == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(identifiers));
            }

            Identifiers = new Collection<TIdentifier>(
                identifiers.ToList()
            );

            IdentifierComparer = EqualityComparer<TIdentifier>.Default;
        }

        public Collection<TIdentifier> Identifiers { get; private set; }

        public EqualityComparer<TIdentifier> IdentifierComparer { get; protected set; }

        protected abstract TIdentifier GetIdentifier(TResult result);

        protected override IEnumerable<TResult> ToEnumerable(Object state, TContext context)
        {
            var items = Parent.ToEnumerable(state, context);

            var byId = items.ToImmutableDictionaryFirstWins(
                item => GetIdentifier(item),
                item => item,
                IdentifierComparer
            );

            foreach (var id in Identifiers)
            {
                TResult value;

                if (byId.TryGetValue(id, out value))
                {
                    yield return value;
                }
                else
                {
                    context.AddFailure(this, id);
                }
            }
        }
    }
}
