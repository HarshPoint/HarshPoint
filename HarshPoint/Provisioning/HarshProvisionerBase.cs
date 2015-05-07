using System;

namespace HarshPoint.Provisioning
{
    public abstract class HarshProvisionerBase
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

        protected virtual void Initialize()
        {
        }

        protected virtual void Complete()
        {
        }

        protected virtual void OnProvisioning()
        {
        }

        protected virtual void OnUnprovisioning()
        {
            if (DeleteUserDataWhenUnprovisioning)
            {
                OnUnprovisioningMayDeleteUserData();
            }
        }

        protected virtual void OnUnprovisioningMayDeleteUserData()
        {
        }
    }
}
