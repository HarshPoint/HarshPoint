using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HarshPoint
{
    public sealed class ProgressBuffer<T> : IProgress<T>
    {
        private readonly List<T> _reports = new List<T>();
        private readonly IReadOnlyCollection<T> _reportsReadOnly;

        public ProgressBuffer()
        {
            _reportsReadOnly = new ReadOnlyCollection<T>(_reports);
        }

        public IReadOnlyCollection<T> Reports => _reportsReadOnly;

        public void Report(T value)
        {
            if (value == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(value));
            }

            _reports.Add(value);
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ProgressBuffer<>));
    }
}
