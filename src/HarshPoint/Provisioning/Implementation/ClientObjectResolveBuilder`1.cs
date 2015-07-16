using Microsoft.SharePoint.Client;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Implementation
{
    public abstract class ClientObjectResolveBuilder<TResult> :
        ClientObjectResolveBuilder<TResult, TResult>
        where TResult : ClientObject
    {
        protected sealed override IEnumerable<TResult> TransformQueryResults(IEnumerable<TResult> results, ClientObjectResolveContext context)
        {
            return results;
        }
    }
}