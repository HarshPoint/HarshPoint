using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Implementation
{
    public abstract class HarshProvisionerContextBase<TSelf>
        : HarshProvisionerContextBase
        where TSelf : HarshProvisionerContextBase<TSelf>
    {
        private Boolean _mayDeleteUserData;

        public override Boolean MayDeleteUserData => _mayDeleteUserData;

        public TSelf AllowDeleteUserData()
        {
            if (MayDeleteUserData)
            {
                return (TSelf)(this);
            }

            var result = Clone();
            result._mayDeleteUserData = true;
            return result;
        }

        public virtual TSelf Clone()
        {
            return (TSelf)MemberwiseClone();
        }

    }
}
