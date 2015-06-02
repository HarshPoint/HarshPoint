using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Implementation
{
    public abstract class HarshProvisionerContextBase
    {
        public abstract Boolean MayDeleteUserData
        {
            get;
        }
    }
}
