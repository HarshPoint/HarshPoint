using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ResolveResultConverter<T> : IEnumerable<T>
    {
        private readonly IEnumerable<Object> _source;
        private readonly ResolveResultConverterStrategy _strategy;

        public ResolveResultConverter(IEnumerable<Object> source)
        {
            if (source == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(source));
            }

            _source = source;
            _strategy = ResolveResultConverterStrategy.ForType(typeof(T));
        }

        public IEnumerator<T> GetEnumerator()
            => _source.Select(_strategy.Convert).Cast<T>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ResolveResultConverter<>));

        private static readonly TypeInfo ResultTypeInfo = typeof(T).GetTypeInfo();
    }
}
