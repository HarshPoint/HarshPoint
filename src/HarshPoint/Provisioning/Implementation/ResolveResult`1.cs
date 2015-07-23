using System.Collections;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ResolveResult<T> : ResolveResultBase, IResolve<T>
    {
        public IEnumerator<T> GetEnumerator()
        {
            return EnumerateResults<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
