using System;
using System.Collections.Immutable;

namespace HarshPoint
{
    public sealed class ProgressComposite<T> : IProgress<T>
    {
        private readonly ImmutableArray<IProgress<T>> _progresses;

        public ProgressComposite(params IProgress<T>[] progresses)
        {
            if (progresses == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(progresses));
            }

            _progresses = ImmutableArray.CreateRange(progresses);
        }

        public void Report(T value)
        {
            foreach (var reporter in _progresses)
            {
                reporter.Report(value);
            }
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ProgressComposite<>));
    }
}
