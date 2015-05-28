using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarshPoint.Tests
{
    internal class HarshTraceTestListener : HarshTraceListener
    {
        private readonly List<HarshTraceEvent> _events = new List<HarshTraceEvent>();

        public override Task Write(HarshTraceEvent e)
        {
            _events.Add(e);
            return HarshTask.Completed;
        }

        public IReadOnlyList<HarshTraceEvent> Events => _events;
    }
}
