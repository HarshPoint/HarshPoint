using HarshPoint.Provisioning.Implementation;
using System;

namespace HarshPoint.Provisioning
{
    public interface IResolveContext
    {
        void AddFailure(Object resolvable, Object identifier);

        HarshProvisionerContextBase ProvisionerContext
        {
            get;
        }
    }
}
