using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ContextStateResolver<TResult> : ResolveBuilder<TResult, IResolveContext>
    {
        protected override Object Initialize(IResolveContext context)
        {
            return null;
        }

        protected override IEnumerable ToEnumerable(Object state, IResolveContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            return context.ProvisionerContext.GetState<TResult>();
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ContextStateResolver<>));
    }
}
