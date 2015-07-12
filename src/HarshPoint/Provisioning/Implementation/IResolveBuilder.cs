using System.Collections;

namespace HarshPoint.Provisioning.Implementation
{
    public interface IResolveBuilder
    {
    }

    public interface IResolveBuilder<in TContext> : IResolveBuilder
        where TContext : HarshProvisionerContextBase
    {
        IEnumerable ToEnumerable(TContext context);
    }
}
