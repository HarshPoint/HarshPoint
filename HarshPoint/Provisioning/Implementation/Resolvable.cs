using System;
using System.Collections.Generic;
using System.Linq;

namespace HarshPoint.Provisioning.Implementation
{
    internal static class Resolvable
    {
        public static T EnsureSingle<T>(Object resolvable, IEnumerable<T> values)
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
                case 1: return values.First();
                case 0: throw Error.InvalidOperation(SR.Resolvable_NoResult, resolvable);
                default: throw Error.InvalidOperation(SR.Resolvable_ManyResults, resolvable);
            }
        }
    }
}
