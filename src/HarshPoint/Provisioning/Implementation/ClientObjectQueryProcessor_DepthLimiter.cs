using System;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Implementation
{
    partial class ClientObjectQueryProcessor
    {
        private sealed class DepthLimiter
        {
            private readonly Dictionary<Type, Int32> _currentDepth
                = new Dictionary<Type, Int32>();

            private readonly ClientObjectQueryProcessor _owner;

            public DepthLimiter(ClientObjectQueryProcessor owner)
            {
                _owner = owner;
            }

            public Int32 MaxDepth => _owner.MaxRecursionDepth;

            public Boolean CanRecurse(Type t)
            {
                if (t == null)
                {
                    throw Logger.Fatal.ArgumentNull(nameof(t));
                }

                return GetCurrentDepth(t) <= MaxDepth;
            }

            public IDisposable Enter(Type t)
            {
                if (t == null)
                {
                    throw Logger.Fatal.ArgumentNull(nameof(t));
                }

                _currentDepth[t] = GetCurrentDepth(t) + 1;

                return new HarshDisposableAction(() =>
                {
                    _currentDepth[t] = _currentDepth[t] - 1;
                });
            }

            private Int32 GetCurrentDepth(Type t)
            {
                var current = 0;
                _currentDepth.TryGetValue(t, out current);
                return current;
            }
        }
    }
}
