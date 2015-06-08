using HarshPoint.Provisioning.Implementation;
using System.Collections.Immutable;

namespace HarshPoint.Provisioning
{
    public class HarshProvisionerResult
    {
        public HarshProvisionerResult()
        {
            ChildResults = ImmutableArray<HarshProvisionerResult>.Empty;
        }

        public ImmutableArray<HarshProvisionerResult> ChildResults
        {
            get;
            internal set;
        }

        public HarshProvisionerBase Provisioner
        {
            get;
            internal set;
        }
    }
}
