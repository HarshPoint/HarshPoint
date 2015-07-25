using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ContextStateResolver<TResult> : ResolveBuilder<TResult, IResolveContext>, IResolveOld<TResult>
    {
        [Obsolete]
        public Task<IEnumerable<TResult>> TryResolveAsync(IResolveContext context)
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            return Task.FromResult(
                context.ProvisionerContext.GetState<TResult>()
            );
        }

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
