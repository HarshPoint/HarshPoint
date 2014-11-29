using System;

namespace HarshPoint.Provisioning
{
    public abstract class HarshPotentiallyDestructiveProvisioner : HarshProvisioner
    {
        public Boolean DeleteUserDataWhenUnprovisioning
        {
            get;
            set;
        }

        protected sealed override void OnUnprovisioning()
        {
            if(DeleteUserDataWhenUnprovisioning)
            {
                OnUnprovisioningMayDeleteUserData();
            }
        }

        protected virtual void OnUnprovisioningMayDeleteUserData()
        {
        }
    }
}
