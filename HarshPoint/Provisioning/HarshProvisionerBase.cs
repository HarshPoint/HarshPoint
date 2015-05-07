using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        protected virtual void OnUnprovisioning()
        {
        }
    }
}
