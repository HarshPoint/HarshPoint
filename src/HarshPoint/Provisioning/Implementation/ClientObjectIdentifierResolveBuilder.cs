using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace HarshPoint.Provisioning.Implementation
{
    internal static class ClientObjectIdentifierResolveBuilder
    {
        public static IEnumerable<TResult> ProcessSelectors<TResult, TIdentifier>(
            IResolveBuilder resolveBuilder,
            TIdentifier identifier,
            IEnumerable<Func<TIdentifier, TResult>> selectors,
            ClientObjectResolveContext context
        )
            where TResult : ClientObject
        {
            TResult result = null;
            Exception exception = null;

            foreach (var selector in selectors)
            {
                try
                {
                    result = selector(identifier);
                    break;
                }
                catch (Exception exc)
                {
                    exception = exc;
                }
            }

            if (result != null)
            {
                yield return result;
            }
            else if (exception != null)
            {
                context.AddFailure(resolveBuilder, identifier);
            }
        }
    }
}
