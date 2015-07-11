using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ResolveResult<T> : IResolve<T>
    {
        private readonly IEnumerable _results;

        public ResolveResult(IEnumerable results)
        {
            _results = results;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _results.Cast<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
