using Microsoft.SharePoint.Client;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HarshPoint.Provisioning.Implementation
{
    public abstract class ClientObjectResolveBuilder<T, TIdentifier, TSelf> :
        ClientObjectResolveBuilder<T, TSelf>
        where T : ClientObject
        where TSelf : ClientObjectResolveBuilder<T, TIdentifier, TSelf>
    {
        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ClientObjectResolveBuilder<,,>));

        protected ClientObjectResolveBuilder(IEnumerable<TIdentifier> identifiers)
        {
            if (identifiers==null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(identifiers));
            }

            Identifiers = new Collection<TIdentifier>(
                identifiers.ToList()
            );
        }

        public Collection<TIdentifier> Identifiers { get; private set; }
    }
}
