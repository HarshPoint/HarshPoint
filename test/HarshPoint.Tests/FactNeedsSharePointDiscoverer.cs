using System.Collections.Generic;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace HarshPoint.Tests
{
    public sealed class FactNeedsSharePointDiscoverer : 
        IXunitTestCaseDiscoverer
    {
        private readonly IMessageSink _diagnosticSink;

        public FactNeedsSharePointDiscoverer(IMessageSink sink)
        {
            _diagnosticSink = sink;
        }

        public IEnumerable<IXunitTestCase> Discover(
            ITestFrameworkDiscoveryOptions discoveryOptions, 
            ITestMethod testMethod, 
            IAttributeInfo factAttribute
        )
        {
            yield return new FactNeedsSharePointTestCase(
                _diagnosticSink, 
                discoveryOptions.MethodDisplayOrDefault(), 
                testMethod
            );
        }
    }
}
