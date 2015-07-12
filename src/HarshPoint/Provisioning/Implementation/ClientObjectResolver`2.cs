using Microsoft.SharePoint.Client;
using System.Linq;
using System.Collections;
using System;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Implementation
{
    public abstract class ClientObjectResolver<T, TSelf> :
        Chain<IClientObjectResolverElement>,
        IClientObjectResolverElement,
        IResolveBuilder<HarshProvisionerContext>
        where T : ClientObject
        where TSelf : ClientObjectResolver<T, TSelf>
    {
        IEnumerable IResolveBuilder<HarshProvisionerContext>.ToEnumerable(HarshProvisionerContext context)
        {
            return Elements.SelectMany(e => e.ToEnumerable(context));
        }

        public abstract IEnumerable<Object> ToEnumerable(HarshProvisionerContext context);
    }
}
