using System;
using System.Collections;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ContextStateResolveBuilder<TResult> : ResolveBuilder<TResult, IResolveContext>
    {
        protected override IEnumerable ToEnumerable(Object state, IResolveContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            return context.ProvisionerContext.GetState<TResult>();
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ContextStateResolveBuilder<>));
    }
}
