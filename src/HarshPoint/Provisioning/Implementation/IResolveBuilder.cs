using System.Collections;

namespace HarshPoint.Provisioning.Implementation
{
    public interface IResolveBuilder
    {
        IEnumerable ToEnumerable(HarshProvisionerContextBase context);
    }
}
