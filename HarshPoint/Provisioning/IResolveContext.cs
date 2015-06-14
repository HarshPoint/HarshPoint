using HarshPoint.Provisioning.Implementation;
using System;

namespace HarshPoint.Provisioning
{
    public interface IResolveContext
    {
        void AddFailure(Object resolvable, Object identifier);

        void ValidateNoFailures();

        HarshProvisionerContextBase ProvisionerContext
        {
            get;
        }
    }

    public static class IResolveContextExtensions
    {
        public static void AddFailure(this IResolveContext context, Object resolvable)
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            if (resolvable==null)
            {
                throw Error.ArgumentNull(nameof(resolvable));
            }

            context.AddFailure(resolvable, null);
        }
    }
}
