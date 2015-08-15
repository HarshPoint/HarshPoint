using System.Collections;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ResolveResult<T> : ResolveResultBase, IResolve<T>
    {
        public IEnumerator<T> GetEnumerator()
        {
            ValidateNoFailures();
            return EnumerateResults<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
