using System;

namespace HarshPoint.Provisioning
{
    /// <summary>
    /// Base class for all client-side object model provisioners that
    /// can potentially destroy user data when unprovisioning.
    /// </summary>
    public abstract class HarshPotentiallyDestructiveProvisioner : HarshProvisioner
    {
        /// <summary>
        /// Value representing whether the provisioner does 
        /// anything at all while unprovisioning.
        /// </summary>
        /// <value>When <c>true</c>, the <see cref="OnUnprovisioningMayDeleteUserData"/>
        /// method is called. Otherwise, the <see cref="Unprovision"/> method 
        /// doesn't do anything.</value>
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
