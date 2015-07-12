using System;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Implementation
{
    public interface IClientObjectResolverElement
    {
        IEnumerable<Object> ToEnumerable(HarshProvisionerContext context);
    }
}