using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Implementation
{
    [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public abstract class Resolvable<T, TContext, TSelf> :
        ResolvableChain,
        IResolvableChainElement<T>,
        IResolve<T>
        where TContext : HarshProvisionerContextBase
        where TSelf : Resolvable<T, TContext, TSelf>
    {
        public TSelf And(IResolvableChainElement<T> other)
        {
            if (other == null)
            {
                throw Error.ArgumentNull(nameof(other));
            }

            return (TSelf)base.And((ResolvableChain)(other));
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        protected abstract Task<IEnumerable<T>> ResolveChainElement(ResolveContext<TContext> context);

        Task<IEnumerable<T>> IResolvableChainElement<T>.ResolveChainElement(IResolveContext context)
            => ResolveChainElement(ValidateContext<TContext>(context));

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        Task<IEnumerable<T>> IResolve<T>.TryResolveAsync(IResolveContext context)
            => ResolveChain<T>(context);
    }
}
