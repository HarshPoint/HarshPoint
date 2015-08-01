using Microsoft.SharePoint.Client;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ClientObjectContextStateResolveBuilder<T> : ClientObjectResolveBuilder<T>
        where T : ClientObject
    {
        protected override IEnumerable<T> CreateObjects(ClientObjectResolveContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }


            return context.ProvisionerContext.GetState<T>();
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ClientObjectContextStateResolveBuilder<>));
    }
}
