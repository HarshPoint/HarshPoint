using System;

namespace HarshPoint.Provisioning
{
    public abstract class HarshProvisionerBase
    {
        protected virtual void Initialize()
        {
        }

        protected virtual void Complete()
        {
        }

        protected virtual void OnProvisioning()
        {
        }

        [NeverDeletesUserData]
        protected virtual void OnUnprovisioning()
        {
        }

    }
}
