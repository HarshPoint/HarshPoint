using System;
using System.Collections;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Implementation
{
    partial class DeferredResolveBuilder<TResult> : IResolve<TResult>, IResolveSingle<TResult>, IResolveSingleOrDefault<TResult>
    {
        TResult IResolveSingle<TResult>.Value { get { throw CannotCall(); } }
        TResult IResolveSingleOrDefault<TResult>.Value { get { throw CannotCall(); } }
        Boolean IResolveSingleOrDefault<TResult>.HasValue { get { throw CannotCall(); } }
        IEnumerator<TResult> IEnumerable<TResult>.GetEnumerator() { throw CannotCall(); }
        IEnumerator IEnumerable.GetEnumerator() { throw CannotCall(); }

        private static Exception CannotCall()
            => Logger.Fatal.InvalidOperation(SR.ResolveBuilder_CannotCallThisMethod);
    }
}
