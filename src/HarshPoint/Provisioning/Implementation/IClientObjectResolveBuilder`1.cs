using Microsoft.SharePoint.Client;
using System;
using System.Linq.Expressions;

namespace HarshPoint.Provisioning.Implementation
{
    public interface IClientObjectResolveBuilder<T> : IResolveBuilder<HarshProvisionerContext>
        where T : ClientObject
    {
        void Include(params Expression<Func<T, Object>>[] retrievals);
    }
}
