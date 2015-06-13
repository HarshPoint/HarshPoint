using HarshPoint.Provisioning.Implementation;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    public interface IResolve<T>
    {
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        Task<IEnumerable<T>> ResolveAsync(IResolveContext context);
    }
}
