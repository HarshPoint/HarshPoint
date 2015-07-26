using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
            _strategy = GetStrategyForType(typeof(T));
        }

        public IEnumerator<T> GetEnumerator()
            => _strategy
                .ConvertResults(_source)
                .Cast<T>()
                .GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        private static ResolveResultConverterStrategy GetStrategyForType(Type type)
        {
            if (type == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(type));
            }

            if (HarshTuple.IsTupleType(type))
            {
                return new ResolveResultConverterStrategyTuple(type);
            }

            return ResolveResultConverterStrategyUnpack.Instance;
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ResolveResultConverter<>));
    }
}
