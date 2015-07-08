using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Implementation
{
    [Obsolete]
    internal sealed class ResolvableContextModification<T> : IResolve<T>
    {
        public ResolvableContextModification(IResolve<T> innerResolvable, Action<IResolveContext> contextModifier)
        {
            if (innerResolvable == null)
            {
                throw Error.ArgumentNull(nameof(innerResolvable));
            }

            if (contextModifier == null)
            {
                throw Error.ArgumentNull(nameof(contextModifier));
            }

            ContextModifier = contextModifier;
            InnerResolvable = innerResolvable;
        }

        public Task<IEnumerable<T>> TryResolveAsync(IResolveContext context)
        {
            ContextModifier(context);

            return InnerResolvable.TryResolveAsync(context);
        }

        private IResolve<T> InnerResolvable
        {
            get;
            set;
        }

        private Action<IResolveContext> ContextModifier
        {
            get;
            set;
        }
    }
}
