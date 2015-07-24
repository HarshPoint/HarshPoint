using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Implementation
{
    [Obsolete]
    public interface IResolvableChainElementOld<T>
    {
        Task<IEnumerable<T>> ResolveChainElementOld(IResolveContext context);
    }

    public interface IResolvableChainElement<out T>
    {
        void InitializeChainElement(IResolveContext context);

        IEnumerable<T> ResolveChainElement();
    }
}
