using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Collections;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ClientObjectContextStateResolver<T> : ResolveBuilder<T, ClientObjectResolveContext>, IResolveOld<T>
        where T : ClientObject
    {
        protected override void InitializeContext(ClientObjectResolveContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            var state = context.ProvisionerContext.GetState<T>();

            foreach (var item in state)
            {
                context.Load(item);
            }

            base.InitializeContext(context);
        }

        protected override Object Initialize(ClientObjectResolveContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            return context.ProvisionerContext.GetState<T>();
        }

        protected override IEnumerable ToEnumerable(Object state, ClientObjectResolveContext context)
        {
            return (IEnumerable)(state);
        }

        [Obsolete]
        public async Task<IEnumerable<T>> TryResolveAsync(IResolveContext context)
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            var state = context.ProvisionerContext.GetState<T>();

            var retrievalContext = context as ClientObjectResolveContext;

            if (retrievalContext != null)
            {
                var clientContext = retrievalContext.ProvisionerContext.ClientContext;
                var retrievals = retrievalContext.GetRetrievals<T>();

                foreach (var item in state)
                {
                    clientContext.Load(item, retrievals);
                }

                await clientContext.ExecuteQueryAsync();
            }

            return state;
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ClientObjectContextStateResolver<>));
    }
}
