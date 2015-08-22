using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HarshPoint.Provisioning.ProgressReporting
{
    public sealed class ProgressBuffer : IProgress<ProgressReport>
    {
        private readonly List<ProgressReport> _reports
            = new List<ProgressReport>();

        private readonly IReadOnlyCollection<ProgressReport> _reportsRo;

        public ProgressBuffer()
        {
            _reportsRo = new ReadOnlyCollection<ProgressReport>(_reports);
        }

        public IReadOnlyCollection<ProgressReport> Reports => _reportsRo;

        public void Report(ProgressReport value)
        {
            if (value == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(value));
            }

            _reports.Add(value);
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext<ProgressBuffer>();
    }
}
