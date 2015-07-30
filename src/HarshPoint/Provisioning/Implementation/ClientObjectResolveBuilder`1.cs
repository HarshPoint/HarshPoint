using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Collections.Immutable;

namespace HarshPoint.Provisioning.Implementation
{
    public abstract class ClientObjectResolveBuilder<TResult> :
        ResolveBuilder<TResult, ClientObjectResolveContext>
        where TResult : ClientObject
    {
        protected sealed override Object Initialize(ClientObjectResolveContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            var objects = CreateObjects(context) ?? new TResult[0];

            if (objects != null)
            {
                foreach (var clientObj in objects)
                {
                    context.Load(clientObj);
                }
            }

            return objects.ToImmutableArray();
        }

        protected sealed override IEnumerable ToEnumerable(Object state, ClientObjectResolveContext context)
            => (IEnumerable)(state);

        protected abstract IEnumerable<TResult> CreateObjects(ClientObjectResolveContext context);

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ClientObjectResolveBuilder<>));
    }
}
