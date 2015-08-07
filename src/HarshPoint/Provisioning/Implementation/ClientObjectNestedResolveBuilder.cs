using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace HarshPoint.Provisioning.Implementation
{
    public abstract class ClientObjectNestedResolveBuilder<TResult, TParent> :
        NestedResolveBuilder<TResult, TParent, ClientObjectResolveContext>
        where TResult : ClientObject
        where TParent : ClientObject
    {
        protected ClientObjectNestedResolveBuilder(
            IResolveBuilder<TParent> parent,
            Expression<Func<TParent, IEnumerable<TResult>>> childrenExpression
        )
            : base(parent)
        {
            if (childrenExpression == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(childrenExpression));
            }

            ChildrenExpression = childrenExpression.ConvertToObject();
            ChildrenSelector = childrenExpression.Compile();
        }

        protected ClientObjectNestedResolveBuilder(
            IResolveBuilder<TParent> parent,
            Expression<Func<TParent, TResult>> childrenExpression
        )
            : base(parent)
        {
            if (childrenExpression == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(childrenExpression));
            }

            ChildrenExpression = childrenExpression.ConvertToObject();

            ChildrenSelector = childrenExpression
                .ConvertToSingleElementArray()
                .Compile();
        }

        protected sealed override void InitializeContextBeforeParent(ClientObjectResolveContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            context.Include(ChildrenExpression);

            base.InitializeContextBeforeParent(context);
        }

        protected sealed override IEnumerable<TResult> SelectChildren(TParent parent)
        {
            if (parent == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(parent));
            }

            return ChildrenSelector(parent);
        }

        private Expression<Func<TParent, Object>> ChildrenExpression
        {
            get; set;
        }

        private Func<TParent, IEnumerable<TResult>> ChildrenSelector
        {
            get; set;
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ClientObjectNestedResolveBuilder<,>));
    }
}
