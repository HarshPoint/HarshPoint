using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
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
