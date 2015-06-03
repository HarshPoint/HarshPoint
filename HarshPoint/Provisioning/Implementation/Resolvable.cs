using System;
using System.Collections.Generic;
using System.Linq;

namespace HarshPoint.Provisioning.Implementation
{
    internal static class Resolvable
    {
        public static T EnsureSingleOrDefault<T>(Object resolvable, IEnumerable<T> values)
        {
            if (resolvable == null)
            {
                throw Error.ArgumentNull(nameof(resolvable));
            }

            if (values == null)
            {
                throw Error.ArgumentNull(nameof(values));
            }

            switch (values.Count())
            {
                case 0: return default(T);
                case 1: return values.First();
                default: throw Error.InvalidOperation(SR.Resolvable_ManyResults, resolvable);
            }
        }
    }
}
