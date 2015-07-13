using System;
using System.Linq.Expressions;

namespace HarshPoint.Provisioning.Implementation
{
    public interface IClientObjectResolveBuilder<T> : IResolveBuilder<HarshProvisionerContext>
    {
        void Include(params Expression<Func<T, Object>>[] retrievals);
    }
}
