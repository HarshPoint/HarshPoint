using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace HarshPoint.Provisioning.Implementation
{
    public sealed class ResolveFailedException : Exception
    {
        public ResolveFailedException()
        {
        }

        public ResolveFailedException(IEnumerable<ResolveFailure> failures)
            : base(FormatFailures(failures))
        {
            Failures = failures.ToImmutableArray();
        }

        public ResolveFailedException(String message)
            : base(message)
        {
        }

        public ResolveFailedException(String message, Exception inner)
            : base(message, inner)
        {
        }

        public IReadOnlyCollection<ResolveFailure> Failures
        {
            get;

        }

        private static String FormatFailures(IEnumerable<ResolveFailure> failures)
        {
            if (failures==null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(failures));
            }

            return String.Join("\n", failures);
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolveFailedException>();
    }
}