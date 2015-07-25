using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ResolveResultConverter<T> : IEnumerable<T>
    {
        private readonly IEnumerable _source;

        public ResolveResultConverter(IEnumerable source)
        {
            _source = source;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _source.Cast<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
