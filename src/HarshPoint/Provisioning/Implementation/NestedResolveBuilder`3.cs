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
        protected NestedResolveBuilder(IResolveBuilder<TParent, TContext> parent)
        {
            if (parent == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(parent));
            }

            Parent = parent;
        }

        public IResolveBuilder<TParent> Parent { get; private set; }

        protected sealed override void InitializeContext(TContext context)
        {
            InitializeContextBeforeParent(context);
            Parent.InitializeContext(context);
        }

        protected virtual void InitializeContextBeforeParent(TContext context)
        {
        }

        protected abstract IEnumerable<TResult> SelectChildren(TParent parent);

        protected override Object Initialize(TContext context)
            => Parent.Initialize(context);

        protected override IEnumerable ToEnumerable(Object state, TContext context)
            => from parent in Parent.ToEnumerable(state, context)
               let unpacked = NestedResolveResult.Unpack<TParent>(parent)

               from child in SelectChildren(unpacked)
               select NestedResolveResult.Pack(child, parent);


        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(NestedResolveBuilder<,,>));
    }
}
