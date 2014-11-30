using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    /// <summary>
    /// Provides common initialization and completion logic for 
    /// classes provisioning SharePoint artifacts.
    /// </summary>
    public abstract class HarshProvisionerBase
    {
        public void Provision()
        {
            try
            {
                Initialize();
                OnProvisioning();
            }
            finally
            {
                Complete();
            }
        }

        public void Unprovision()
        {
            try
            {
                Initialize();
                OnUnprovisioning();
            }
            finally
            {
                Complete();
            }
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
        }
    }
}
