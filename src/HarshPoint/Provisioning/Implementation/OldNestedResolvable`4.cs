using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Implementation
{
    [Obsolete]
    [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public abstract class OldNestedResolvable<T1, T2, TContext, TSelf> :
        ResolvableChain,

        IResolvableChainElementOld<IGrouping<T1, T2>>,
        IResolvableChainElementOld<Tuple<T1, T2>>,
        IResolvableChainElementOld<T2>,

        IResolveOld<IGrouping<T1, T2>>,
        IResolveOld<Tuple<T1, T2>>,
        IResolveOld<T2>

        where TContext : HarshProvisionerContextBase
        where TSelf : OldNestedResolvable<T1, T2, TContext, TSelf>
    {
        protected OldNestedResolvable(IResolveOld<T1> parent)
        {
            if (parent == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(parent));
            }

            Parents = parent;
        }

        public IResolveOld<T1> Parents
        {
            get;
            private set;
        }

        public TSelf And(IResolvableChainElementOld<IGrouping<T1, T2>> other)
        {
            if (other == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(other));
            }

            return (TSelf)And((ResolvableChain)(other));
        }

        public TSelf And(IResolvableChainElementOld<Tuple<T1, T2>> other)
        {
            if (other == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(other));
            }

            return (TSelf)And((ResolvableChain)(other));
        }

        public TSelf And(IResolvableChainElementOld<T2> other)
        {
            if (other == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(other));
            }

            return (TSelf)And((ResolvableChain)(other));
        }

        protected abstract Task<IEnumerable<T2>> ResolveChainElement(ResolveContext<TContext> context, T1 parent);

        Task<IEnumerable<IGrouping<T1, T2>>> IResolvableChainElementOld<IGrouping<T1, T2>>.ResolveChainElementOld(IResolveContext context)
            => ResolveChainElement(context);

        async Task<IEnumerable<Tuple<T1, T2>>> IResolvableChainElementOld<Tuple<T1, T2>>.ResolveChainElementOld(IResolveContext context)
            => from grouping in (await ResolveChainElement(context))
               from item in grouping
               select Tuple.Create(grouping.Key, item);

        async Task<IEnumerable<T2>> IResolvableChainElementOld<T2>.ResolveChainElementOld(IResolveContext context)
            => from grouping in (await ResolveChainElement(context))
               from item in grouping
               select item;

        Task<IEnumerable<IGrouping<T1, T2>>> IResolveOld<IGrouping<T1, T2>>.TryResolveAsync(IResolveContext context)
            => ResolveChainOld<IGrouping<T1, T2>>(context);

        Task<IEnumerable<Tuple<T1, T2>>> IResolveOld<Tuple<T1, T2>>.TryResolveAsync(IResolveContext context)
            => ResolveChainOld<Tuple<T1, T2>>(context);

        Task<IEnumerable<T2>> IResolveOld<T2>.TryResolveAsync(IResolveContext context)
            => ResolveChainOld<T2>(context);

        private async Task<IEnumerable<IGrouping<T1, T2>>> ResolveChainElement(IResolveContext context)
        {
            var typedContext = ValidateContext<TContext>(context);
            var parents = await Parents.TryResolveAsync(context);

            return await parents.SelectSequentially(
                async parent => HarshGrouping.Create(
                    parent,
                    await ResolveChainElement(typedContext, parent)
                )
            );
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(OldNestedResolvable<,,,>));
    }
}
