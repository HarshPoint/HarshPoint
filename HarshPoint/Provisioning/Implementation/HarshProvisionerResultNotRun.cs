using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class HarshProvisionerResultNotRun : HarshProvisionerResult
    {
        public HarshProvisionerResultNotRun(HarshProvisionerBase provisioner)
            : base(provisioner)
        {
        }
    }
}
