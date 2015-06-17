using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ClientObjectContextStateResolver<T> : IResolve<T>
        where T : ClientObject
    {
        public async Task<IEnumerable<T>> TryResolveAsync(IResolveContext context)
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            var state = context.ProvisionerContext.GetState<T>();

            var retrievalContext = context as ClientObjectResolveContext<T>;

            if (retrievalContext != null)
            {
                var clientContext = retrievalContext.ProvisionerContext.ClientContext;

                foreach (var item in state)
                {
                    clientContext.Load(item, retrievalContext.Retrievals.ToArray());
                }

                await clientContext.ExecuteQueryAsync();
            }

            return state;
        }
    }
}
