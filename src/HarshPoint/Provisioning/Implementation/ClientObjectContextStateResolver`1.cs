using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ClientObjectContextStateResolver<T> : IResolveOld<T>
        where T : ClientObject
    {
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
    }
}
