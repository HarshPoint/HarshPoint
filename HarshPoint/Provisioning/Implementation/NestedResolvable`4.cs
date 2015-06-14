using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Implementation
{
    [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public abstract class NestedResolvable<T1, T2, TContext, TSelf> :
        ResolvableChain,
        
        IResolvableChainElement<IGrouping<T1, T2>>,
        IResolve<IGrouping<T1, T2>>,    
        
        IResolvableChainElement<T2>,
        IResolve<T2>

        where TContext : HarshProvisionerContextBase
        where TSelf : NestedResolvable<T1, T2, TContext, TSelf>
    {
        protected NestedResolvable(IResolve<T1> parent)
        {
            if (parent == null)
            {
                throw Error.ArgumentNull(nameof(parent));
            }

            Parents = parent;
        }

        public IResolve<T1> Parents
        {
            get;
            private set;
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public TSelf And(IResolvableChainElement<IGrouping<T1, T2>> other)
        {
            if (other == null)
            {
                throw Error.ArgumentNull(nameof(other));
            }

            return (TSelf)And((ResolvableChain)(other));
        }
        
        public TSelf And(IResolvableChainElement<T2> other)
        {
            if (other == null)
            {
                throw Error.ArgumentNull(nameof(other));
            }

            return (TSelf)And((ResolvableChain)(other));
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        protected abstract Task<IEnumerable<T2>> ResolveChainElement(ResolveContext<TContext> context, T1 parent);

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        async Task<IEnumerable<T2>> IResolvableChainElement<T2>.ResolveChainElement(IResolveContext context)
            => (await ResolveChainElement(context)).SelectMany(g => g);

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        Task<IEnumerable<IGrouping<T1, T2>>> IResolvableChainElement<IGrouping<T1, T2>>.ResolveChainElement(IResolveContext context)
            => ResolveChainElement(context);

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        Task<IEnumerable<T2>> IResolve<T2>.TryResolveAsync(IResolveContext context)
            => ResolveChain<T2>(context);

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        Task<IEnumerable<IGrouping<T1, T2>>> IResolve<IGrouping<T1, T2>>.TryResolveAsync(IResolveContext context)
            => ResolveChain<IGrouping<T1, T2>>(context);
            
        private async Task<IEnumerable<IGrouping<T1, T2>>> ResolveChainElement(IResolveContext context)
        {
            var typedContext = ValidateContext<TContext>(context);
            var parents = await Parents.TryResolveAsync(context);

            return await parents.SelectSequentially(
                p => ResolveChildren(typedContext, p)
            );
        }

        private async Task<IGrouping<T1, T2>> ResolveChildren(ResolveContext<TContext> context, T1 parent)
        {
            return ResolvedGrouping.Create(
                parent,
                await ResolveChainElement(context, parent)
            );
        }
    }
}
