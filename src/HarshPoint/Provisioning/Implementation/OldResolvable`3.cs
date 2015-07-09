using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Implementation
{
    [Obsolete]
    [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public abstract class OldResolvable<T, TProvisionerContext, TSelf> :
        ResolvableChain,

        IResolvableChainElementOld<T>,
        IResolveOld<T>
    
        where TProvisionerContext : HarshProvisionerContextBase
        where TSelf : OldResolvable<T, TProvisionerContext, TSelf>
    {
        private static readonly HarshLogger Logger = HarshLog.ForContext<OldResolvable<T, TProvisionerContext, TSelf>>();

        [Obsolete]
        public TSelf And(IResolvableChainElementOld<T> other)
        {
            if (other == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(other));
            }

            return (TSelf)base.And((ResolvableChain)(other));
        }

        [Obsolete]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        protected abstract Task<IEnumerable<T>> ResolveChainElementOld(ResolveContext<TProvisionerContext> context);

        [Obsolete]
        public Task<IEnumerable<T>> TryResolveAsync(IResolveContext context)
            => ResolveChainOld<T>(context);

        [Obsolete]
        Task<IEnumerable<T>> IResolvableChainElementOld<T>.ResolveChainElementOld(IResolveContext context)
            => ResolveChainElementOld(
                ValidateContext<TProvisionerContext>(context)
            );
    }
}
