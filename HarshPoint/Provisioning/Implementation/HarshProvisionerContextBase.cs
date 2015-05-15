using System;

namespace HarshPoint.Provisioning.Implementation
{
    public abstract class HarshProvisionerContextBase
    {
        public Boolean MayDeleteUserData
        {
            get;
            set;
        }

        public virtual HarshProvisionerContextBase Clone()
        {
            return (HarshProvisionerContextBase)MemberwiseClone();
        }
    }
}
