using Microsoft.SharePoint.Client;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Implementation
{
    internal static class ClientObjectCachedResolveResultProcessor
    {
        public static void InitializeCached<TResult>(
            ClientObjectResolveContext context, 
            IEnumerable<TResult> items
        )
            where TResult : ClientObject
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            if (items == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(items));
            }

            foreach (var item in items)
            {
                context.Load(item);
            }
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ClientObjectCachedResolveResultProcessor));
    }
}
