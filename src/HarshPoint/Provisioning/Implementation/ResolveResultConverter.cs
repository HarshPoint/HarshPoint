using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ResolveResultConverter<T> : IEnumerable<T>
    {
        private readonly IEnumerable<Object> _source;

        public ResolveResultConverter(IEnumerable<Object> source)
        {
            if (source == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(source));
            }

            _source = source;
        }

        public IEnumerator<T> GetEnumerator()
            => Strategy
                .ConvertResults(_source)
                .Cast<T>()
                .GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ResolveResultConverter<>));

        private static readonly ResolveResultConverterStrategy Strategy
            = ResolveResultConverterStrategy.GetStrategyForType(typeof(T));
    }
}
