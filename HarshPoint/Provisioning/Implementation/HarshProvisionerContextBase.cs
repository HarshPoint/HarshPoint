using System;
using System.Collections.Immutable;

namespace HarshPoint.Provisioning.Implementation
{
    public abstract class HarshProvisionerContextBase
    {
        public abstract Boolean MayDeleteUserData
        {
            get;
        }

        public abstract IImmutableStack<Object> StateStack
        {
            get;
        }
    }
}
