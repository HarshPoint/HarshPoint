using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HarshPoint.Tests
{
    public class HarshTraceSourceTests
    {
        public void Writes_source_name_and_value()
        {
            AssertTraceResults(
                () => new HarshTraceSource("source").WriteInfo("test"),
                "source: test"
            );
        }

        private void AssertTraceResults(Action action, params String[] expected)
        {
            var listener = new HarshTraceTestListener();

            HarshTrace.AddListener(listener);

            try
            {
                action();
            }
            finally
            {
                HarshTrace.RemoveListener(listener);
            }

            Assert.Equal(expected.Length, listener.Events.Count);

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expected[i], listener.Events[i].Message);
            }
        }
    }
}
