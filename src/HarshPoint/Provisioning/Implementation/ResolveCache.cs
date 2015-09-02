using System;
using System.Collections;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ResolveCache
    {
        private readonly Dictionary<IResolveBuilder, IEnumerable> _cache
            = new Dictionary<IResolveBuilder, IEnumerable>();

        public IEnumerable TryGetValue(IResolveBuilder resolveBuilder)
        {
            if (resolveBuilder == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(resolveBuilder));
            }

            return _cache.GetValueOrDefault(resolveBuilder);
        }

        public void SetValue(IResolveBuilder resolveBuilder, IEnumerable value)
        {
            if (resolveBuilder == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(resolveBuilder));
            }

            if (value == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(value));
            }

            _cache[resolveBuilder] = value;
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ResolveCache));
    }
}