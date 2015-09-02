using System;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ResolveCache
    {
        private readonly Dictionary<IResolveBuilder, Object> _cache
            = new Dictionary<IResolveBuilder, Object>();

        public ResolveCache()
        {
        }

        public Object TryGetValue(IResolveBuilder resolveBuilder)
        {
            if (resolveBuilder == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(resolveBuilder));
            }

            return _cache.GetValueOrDefault(resolveBuilder);
        }

        public void SetValue(IResolveBuilder resolveBuilder, Object result)
        {
            if (resolveBuilder == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(resolveBuilder));
            }

            _cache[resolveBuilder] = result;
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ResolveCache));
    }
}