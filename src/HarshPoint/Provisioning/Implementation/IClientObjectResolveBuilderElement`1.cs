using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace HarshPoint.Provisioning.Implementation
{
    public interface IClientObjectResolveBuilderElement<T>
        where T : ClientObject
    {
        void Include(params Expression<Func<T, Object>>[] retrievals);
        IEnumerable<T> ToEnumerable(HarshProvisionerContext context);
    }
}