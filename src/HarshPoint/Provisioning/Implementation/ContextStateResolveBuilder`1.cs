using System;
using System.Collections;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ContextStateResolveBuilder<TResult> : ResolveBuilder<TResult, ResolveContext>
    {
        protected override IEnumerable ToEnumerable(Object state, ResolveContext context)
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
