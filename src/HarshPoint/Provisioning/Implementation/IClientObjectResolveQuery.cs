using Microsoft.SharePoint.Client;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Implementation
{
    internal interface IClientObjectResolveQuery<T, TIdentifier> 
        where T : ClientObject
    {
        IEqualityComparer<TIdentifier> IdentifierComparer { get; }
    }
}