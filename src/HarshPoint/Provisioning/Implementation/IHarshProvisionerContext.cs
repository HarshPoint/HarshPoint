using System;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Implementation
{
    public interface IHarshProvisionerContext
    {
        IEnumerable<T> GetState<T>();

        IEnumerable<Object> GetState(Type type);

        void ReportProgress(ProgressReport value);

        Boolean MayDeleteUserData { get; }
    }
}
