using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HarshPoint.Provisioning.Implementation
{
    public abstract partial class NestedResolveBuilder<TResult, TParent, TContext> :
        ResolveBuilder<TResult, TContext>
        where TContext : class, IResolveContext
    {
        protected NestedResolveBuilder(IResolveBuilder<TParent> parent)
        {
            if (parent == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(parent));
            }

            Parent = parent;
        }

        public IResolveBuilder<TParent> Parent { get; }

        protected sealed override void InitializeContext(TContext context)
        {
            InitializeContextBeforeParent(context);
            Parent.InitializeContext(context);
        }

        protected virtual void InitializeContextBeforeParent(TContext context)
        {
        }

        protected abstract IEnumerable<TResult> SelectChildren(TParent parent);

        protected sealed override Object Initialize(TContext context)
            => Parent.Initialize(context);

        protected sealed override IEnumerable ToEnumerable(Object state, TContext context)
            => from parent in Parent.ToEnumerable(context, state)
               let unpacked = NestedResolveResult.Unpack<TParent>(parent)

               from child in SelectChildren(unpacked)
               select NestedResolveResult.Pack(child, parent);

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(NestedResolveBuilder<,,>));
    }
}
