using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Implementation
{
    public interface IClientObjectResolveBuilderElement<T>
        where T : ClientObject
    {
        IEnumerable<T> ToEnumerable(HarshProvisionerContext context);
    }
}