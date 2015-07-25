using System;

namespace HarshPoint.Provisioning.Implementation
{
    public abstract class NestedResolveBuilder<TResult, TParent, TContext> :
        ResolveBuilder<TResult, TContext>
        where TContext : class, IResolveContext
    {
        protected NestedResolveBuilder(IResolveBuilder<TParent, TContext> parent)
        {
            if (parent == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(parent));
            }

            Parent = parent;
        }

        public IResolveBuilder<TParent, TContext> Parent { get; private set; }

        protected sealed override void InitializeContext(TContext context)
        {
            InitializeContextBeforeParent(context);
            Parent.InitializeContext(context);
        }

        protected virtual void InitializeContextBeforeParent(TContext context)
        {
        }

        protected override Object Initialize(TContext context)
        {
            return Parent.Initialize(context);
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(NestedResolveBuilder<,,>));
    }
}
