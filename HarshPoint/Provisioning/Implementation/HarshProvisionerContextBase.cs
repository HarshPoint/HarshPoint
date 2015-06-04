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

        public HarshProvisionerContextBase PushState(Object state)
        {
            return PushStateCore(state);
        }

        protected abstract HarshProvisionerContextBase PushStateCore(Object state);
    }
}
