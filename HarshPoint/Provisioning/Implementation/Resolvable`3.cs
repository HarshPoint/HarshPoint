using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Implementation
{
    [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public abstract class Resolvable<T, TContext, TSelf> :
        ResolvableChain,
        IResolvableChainElement<T>,
        IResolve<T>,
        IResolveSingle<T>
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
        protected abstract Task<IEnumerable<T>> ResolveChainElement(TContext context);

        Task<IEnumerable<T>> IResolvableChainElement<T>.ResolveChainElement(HarshProvisionerContextBase context)
            => ResolveChainElement(ValidateContext<TContext>(context));

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        Task<IEnumerable<T>> IResolve<T>.ResolveAsync(HarshProvisionerContextBase context)
            => ResolveChain<T>(context);

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        Task<T> IResolveSingle<T>.ResolveSingleOrDefaultAsync(HarshProvisionerContextBase context)
            => ResolveChainSingleOrDefault<T>(context);
    }
}
