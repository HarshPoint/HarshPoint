
using System.Collections.Generic;
using System.Linq;

namespace HarshPoint.Provisioning
{
    public sealed class HarshCompositeProvisioner : HarshProvisioner
    {
        public HarshCompositeProvisioner()
        {
            Provisioners = new HarshProvisionerCollection<HarshProvisioner>(
                p => { p.Context = Context; return p; }
            );
        }

        protected override void OnProvisioning()
        {
            Provisioners.Provision();
        }

        protected override void OnUnprovisioning()
        {
            Provisioners.Unprovision();
        }

        public HarshProvisionerCollection<HarshProvisioner> Provisioners
        {
            get;
            private set;
        }

        public static HarshProvisioner FromSequence(IEnumerable<HarshProvisioner> provisioners)
        {
            if (provisioners == null)
            {
                throw Error.ArgumentNull("provisioners");
            }

            var array = provisioners.ToArray();

            if (array.Length == 0)
            {
                throw Error.ArgumentOutOfRange("provisioners", SR.HarshCompositeProvisioner_SequenceEmpty);
            }

            if (array.Length == 1)
            {
                return array[0];
            }

            var result = new HarshCompositeProvisioner();

            foreach (var item in array)
            {
                result.Provisioners.Add(item);
            }

            return result;
        }
    }
}
