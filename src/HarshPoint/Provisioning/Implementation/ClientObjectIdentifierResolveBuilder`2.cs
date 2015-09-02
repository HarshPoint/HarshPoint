using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Collections;
using System.Linq;

namespace HarshPoint.Provisioning.Implementation
{
    public abstract class ClientObjectIdentifierResolveBuilder<TResult, TIdentifier> :
        IdentifierResolveBuilder<TResult, ClientObjectResolveContext, TIdentifier>
        where TResult : ClientObject
    {
        protected ClientObjectIdentifierResolveBuilder(
            IResolveBuilder<TResult> parent,
            IEnumerable<TIdentifier> identifiers,
            Expression<Func<TResult, TIdentifier>> identifierExpression
        )
            : this(parent, identifiers, null, identifierExpression)
        {
        }

        protected ClientObjectIdentifierResolveBuilder(
            IResolveBuilder<TResult> parent,
            IEnumerable<TIdentifier> identifiers,
            IEqualityComparer<TIdentifier> identifierComparer,
            Expression<Func<TResult, TIdentifier>> identifierExpression
        )
        :
            base(parent, identifiers, identifierComparer)
        {
            if (identifierExpression == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(identifierExpression));
            }

            IdentifierExpression = identifierExpression.ConvertToObject();
            IdentifierSelector = identifierExpression.Compile();
        }

        protected sealed override TIdentifier GetIdentifierFromItem(Object item)
            => base.GetIdentifierFromItem(item);

        protected sealed override TIdentifier GetIdentifierFromNested(NestedResolveResult nested)
            => base.GetIdentifierFromNested(nested);

        protected sealed override TIdentifier GetIdentifier(TResult result)
            => IdentifierSelector(result);

        protected override void InitializeCached(
            ClientObjectResolveContext context, 
            IEnumerable enumerable
        )
        {
            ClientObjectCachedResolveResultProcessor.InitializeCached(
                context,
                enumerable.Cast<TResult>()
            );
        }

        protected sealed override void InitializeContextBeforeParent(ClientObjectResolveContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            context.Include(
                IdentifierExpression
            );

            base.InitializeContextBeforeParent(context);
        }

        private Expression<Func<TResult, Object>> IdentifierExpression
        {
            get;
        }

        private Func<TResult, TIdentifier> IdentifierSelector
        {
            get;
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ClientObjectIdentifierResolveBuilder<,>));
    }
}
