using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Implementation
{
    public abstract class HarshProvisionerContextBase
    {
        public abstract Boolean MayDeleteUserData
        {
            get;
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public Task<IEnumerable<T>> ResolveAsync<T>(IResolve<T> resolver)
        {
            if (resolver == null)
            {
                throw Error.ArgumentNull(nameof(resolver));
            }

            return resolver.ResolveAsync(this);
        }

        public Task<T> ResolveSingleAsync<T>(IResolveSingle<T> resolver)
        {
            if (resolver == null)
            {
                throw Error.ArgumentNull(nameof(resolver));
            }

            return resolver.ResolveSingleAsync(this);
        }
    }
}
