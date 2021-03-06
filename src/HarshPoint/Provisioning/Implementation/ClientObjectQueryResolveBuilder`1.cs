﻿using Microsoft.SharePoint.Client;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Implementation
{
    public abstract class ClientObjectQueryResolveBuilder<TResult> :
        ClientObjectQueryResolveBuilder<TResult, TResult>
        where TResult : ClientObject
    {
        protected sealed override IEnumerable<TResult> TransformQueryResults(IEnumerable<TResult> results, ClientObjectResolveContext context)
            => results;
    }
}