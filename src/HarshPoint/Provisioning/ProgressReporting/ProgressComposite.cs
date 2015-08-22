using System;
using System.Collections.Immutable;

namespace HarshPoint.Provisioning.ProgressReporting
{
    public sealed class ProgressComposite : IProgress<ProgressReport>
    {
        private readonly ImmutableArray<IProgress<ProgressReport>> _progresses;

        public ProgressComposite(params IProgress<ProgressReport>[] progresses)
        {
            if (progresses == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(progresses));
            }

            _progresses = ImmutableArray.CreateRange(progresses);
        }

        public void Report(ProgressReport value)
        {
            foreach (var reporter in _progresses)
            {
                reporter.Report(value);
            }
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext<ProgressComposite>();
    }
}
