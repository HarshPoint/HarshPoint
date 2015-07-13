using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HarshPoint.Provisioning.Implementation
{
    public interface IClientObjectResolveBuilderElement<out T>
    {
        Object LoadQuery(ResolveContext<HarshProvisionerContext> context);

        IEnumerable<T> TransformQueryResults(Object state, ResolveContext<HarshProvisionerContext> context);
    }
}