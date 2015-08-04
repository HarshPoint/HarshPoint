using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace HarshPoint.Provisioning.Implementation
{
    public abstract class IdentifierResolveBuilder<TResult, TContext, TIdentifier> :
        ResolveBuilder<TResult, TContext>
        where TContext : class, IResolveContext
    {
        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(IdentifierResolveBuilder<,,>));

        protected IdentifierResolveBuilder(
            IResolveBuilder<TResult> parent,
            IEnumerable<TIdentifier> identifiers
        )
            : this(parent, identifiers, null)
        {
        }

        protected IdentifierResolveBuilder(
            IResolveBuilder<TResult> parent,
            IEnumerable<TIdentifier> identifiers,
            IEqualityComparer<TIdentifier> identifierComparer
        )
        {
            if (parent == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(parent));
            }

            if (identifiers == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(identifiers));
            }

            if (identifierComparer == null)
            {
                identifierComparer = EqualityComparer<TIdentifier>.Default;
            }

            Parent = parent;

            Identifiers = ImmutableHashSet.CreateRange(
                identifierComparer, identifiers
            );
        }

        public IResolveBuilder<TResult> Parent { get; private set; }

        public ImmutableHashSet<TIdentifier> Identifiers { get; private set; }

        public IEqualityComparer<TIdentifier> IdentifierComparer => Identifiers.KeyComparer;

        protected sealed override void InitializeContext(TContext context)
        {
            InitializeContextBeforeParent(context);
            Parent.InitializeContext(context);
        }

        protected virtual void InitializeContextBeforeParent(TContext context)
        {
        }

        protected sealed override Object Initialize(TContext context)
            => Parent.Initialize(context);

        protected virtual TIdentifier GetIdentifier(TResult result)
        {
            throw Logger.Fatal.NotImplemented();
        }

        protected virtual TIdentifier GetIdentifierFromNested(NestedResolveResult nested)
        {
            if (nested == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(nested));
            }

            return GetIdentifier((TResult)nested.Value);
        }

        protected virtual TIdentifier GetIdentifierFromItem(Object item)
        {
            if (item == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(item));
            }

            var nested = (item as NestedResolveResult);

            if (nested != null)
            {
                return GetIdentifierFromNested(nested);
            }

            return GetIdentifier((TResult)(item));
        }

        protected sealed override IEnumerable ToEnumerable(Object state, TContext context)
        {
            var items = Parent.ToEnumerable(context, state);

            var byId = items.ToImmutableDictionaryFirstWins(
                item => GetIdentifierFromItem(item),
                item => item,
                IdentifierComparer
            );

            foreach (var id in Identifiers)
            {
                Object value;

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
