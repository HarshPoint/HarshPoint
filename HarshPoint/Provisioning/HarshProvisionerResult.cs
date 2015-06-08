using HarshPoint.Provisioning.Implementation;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace HarshPoint.Provisioning
{
    public class HarshProvisionerResult
    {
        public HarshProvisionerResult(HarshProvisionerBase provisioner)
            : this(provisioner, null)
        {
        }

        public HarshProvisionerResult(HarshProvisionerBase provisioner, IEnumerable<HarshProvisionerResult> childResults)
        {
            if (provisioner == null)
            {
                throw Error.ArgumentNull(nameof(provisioner));
            }

            if (childResults == null)
            {
                childResults = ImmutableList<HarshProvisionerResult>.Empty;
            }

            ChildResults = childResults.ToImmutableList();
            Provisioner = provisioner;
        }

        public IImmutableList<HarshProvisionerResult> ChildResults
        {
            get;
            private set;
        }

        public HarshProvisionerBase Provisioner
        {
            get;
            private set;
        }
    }
}
