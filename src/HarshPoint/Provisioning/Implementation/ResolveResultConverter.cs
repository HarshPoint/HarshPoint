using System;
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
            if (source == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(source));
            }

            _source = source;
        }

        public IEnumerator<T> GetEnumerator()
            => _source
                .Cast<Object>()
                .Select(NestedResolveResult.Unpack<T>)
                .GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ResolveResultConverter<>));
    }
}
