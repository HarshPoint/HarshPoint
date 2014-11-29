using Microsoft.SharePoint.Client;
using System;

namespace HarshPoint.Provisioning
{
    public class HarshFieldProvisioner : HarshPotentiallyDestructiveProvisioner
    {
        public Guid FieldId
        {
            get;
            set;
        }

        public String InternalName
        {
            get;
            set;
        }

        public List List
        {
            get;
            set;
        }

        public String StaticName
        {
            get;
            set;
        }

        protected override void OnProvisioning()
        {
            base.OnProvisioning();
        }
    }
}
