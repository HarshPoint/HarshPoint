using System.Collections.Generic;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace HarshPoint.Server.Tests
{
    public class FarmDependentTestCaseDiscoverer : IXunitTestCaseDiscoverer
    {
        private readonly IMessageSink _diagnosticSink;

        public FarmDependentTestCaseDiscoverer(IMessageSink diagnosticMessageSink)
        {
            _diagnosticSink = diagnosticMessageSink;
        }

        public IEnumerable<IXunitTestCase> Discover(
            ITestFrameworkDiscoveryOptions discoveryOptions,
            ITestMethod testMethod,
            IAttributeInfo factAttribute
            )
        {
            yield return new FarmDependentTestCase(_diagnosticSink, discoveryOptions.MethodDisplayOrDefault(), testMethod);
        }
    }
}
