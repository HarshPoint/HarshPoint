using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace HarshPoint.Provisioning.Implementation
{
    public interface IHarshProvisionerContext
    {
        IEnumerable<T> GetState<T>();

        IEnumerable<Object> GetState(Type type);

        IProvisioningSession Session { get; }

        IImmutableList<IProvisioningSessionInspector> SessionInspectors { get; }

        Boolean MayDeleteUserData { get; }
    }
}
