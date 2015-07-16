using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;

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
            : this(parent, identifiers, null)
        {
        }

        protected IdentifierResolveBuilder(
            IResolveBuilder<TResult, TContext> parent,
            IEnumerable<TIdentifier> identifiers,
            IEqualityComparer<TIdentifier> identifierComparer
        )
            : base(parent)
        {
            if (identifiers == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(identifiers));
            }

            if (identifierComparer == null)
            {
                identifierComparer = EqualityComparer<TIdentifier>.Default;
            }

            Identifiers = new HashSet<TIdentifier>(
                identifiers, identifierComparer
            );
        }

        public HashSet<TIdentifier> Identifiers { get; private set; }

        public IEqualityComparer<TIdentifier> IdentifierComparer => Identifiers.Comparer;

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
