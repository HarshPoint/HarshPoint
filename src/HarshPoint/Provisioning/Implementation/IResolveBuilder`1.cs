using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Implementation
{
    public interface IResolveBuilder
    {
        IEnumerable ToEnumerable(HarshProvisionerContextBase context);
    }
}
