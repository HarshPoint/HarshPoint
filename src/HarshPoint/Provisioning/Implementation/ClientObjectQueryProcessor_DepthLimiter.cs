using System;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Implementation
{
    partial class ClientObjectQueryProcessor
    {
        private sealed class DepthLimiter
        {
            private readonly Int32 _maxDepth;

            private readonly Dictionary<Type, Int32> _remainingDepth
                = new Dictionary<Type, Int32>();

            public DepthLimiter(Int32 maxDepth)
            {
                _maxDepth = maxDepth;
            }

            public Boolean CanRecurse(Type t)
            {
                if (t == null)
                {
                    throw Logger.Fatal.ArgumentNull(nameof(t));
                }

                return GetRemainingDepth(t) > 0;
            }

            public IDisposable Enter(Type t)
            {
                if (t == null)
                {
                    throw Logger.Fatal.ArgumentNull(nameof(t));
                }


                _remainingDepth[t] = GetRemainingDepth(t) - 1;

                return new HarshDisposableAction(
                    () => _remainingDepth[t] = _remainingDepth[t] + 1
                );
            }

            private Int32 GetRemainingDepth(Type t)
            {
                var remaining = 0;

                if (!_remainingDepth.TryGetValue(t, out remaining))
                {
                    remaining = _maxDepth + 1;
                }

                return remaining;
            }
        }
    }
}
