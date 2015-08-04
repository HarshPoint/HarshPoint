using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace HarshPoint.Provisioning.Implementation
{
    public abstract class ClientObjectIdentifierResolveBuilder<TResult, TIdentifier> :
        IdentifierResolveBuilder<TResult, ClientObjectResolveContext, TIdentifier>
        where TResult : ClientObject
    {
        protected ClientObjectIdentifierResolveBuilder(
            IResolveBuilder<TResult> parent, 
            IEnumerable<TIdentifier> identifiers
        ) 
            : base(parent, identifiers)
        {
        }

        protected ClientObjectIdentifierResolveBuilder(
            IResolveBuilder<TResult> parent,
            IEnumerable<TIdentifier> identifiers,
            Expression<Func<TResult, TIdentifier>> identifierExpression
        )
            : base(parent, identifiers)
        {
            IdentifierExpression = identifierExpression;
            IdentifierSelector = identifierExpression?.Compile();
        }

        protected ClientObjectIdentifierResolveBuilder(
            IResolveBuilder<TResult> parent, 
            IEnumerable<TIdentifier> identifiers, 
            IEqualityComparer<TIdentifier> identifierComparer
        ) 
        : 
            base(parent, identifiers, identifierComparer)
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
            IdentifierExpression = identifierExpression;
            IdentifierSelector = identifierExpression?.Compile();

        }

        protected Expression<Func<TResult, TIdentifier>> IdentifierExpression
        {
            get; private set;
        }

        protected Func<TResult, TIdentifier> IdentifierSelector
        {
            get; private set;
        }

        protected override void InitializeContextBeforeParent(ClientObjectResolveContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            if (IdentifierExpression != null)
            {
                context.Include(
                    IdentifierExpression.ConvertToObject()
                );
            }

            base.InitializeContextBeforeParent(context);
        }

        protected override TIdentifier GetIdentifier(TResult result)
        {
            if (IdentifierSelector != null)
            {
                return IdentifierSelector(result);
            }

            return base.GetIdentifier(result);
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ClientObjectIdentifierResolveBuilder<,>));
    }
}
