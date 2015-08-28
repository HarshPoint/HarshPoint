using System.Collections.Generic;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace HarshPoint.Tests
{
    public sealed class TheoryNeedsSharePointDiscoverer : 
        IXunitTestCaseDiscoverer
    {
        private readonly IMessageSink _diagnosticSink;
        private readonly TheoryDiscoverer _theoryDiscoverer;

        public TheoryNeedsSharePointDiscoverer(IMessageSink sink)
        {
            _diagnosticSink = sink;
            _theoryDiscoverer = new TheoryDiscoverer(sink);
        }

        public IEnumerable<IXunitTestCase> Discover(
            ITestFrameworkDiscoveryOptions discoveryOptions, 
            ITestMethod testMethod, 
            IAttributeInfo factAttribute
        )
        {
            var cases = _theoryDiscoverer.Discover(
                discoveryOptions, testMethod, factAttribute
            );

            var defaultMethodDisplay 
                = discoveryOptions.MethodDisplayOrDefault();

            foreach (var tc in cases)
            {
                if (tc is XunitTheoryTestCase)
                {
                    yield return new TheoryNeedsSharePointTestCase(
                        _diagnosticSink,
                        defaultMethodDisplay,
                        tc.TestMethod
                    );
                }
                else
                {
                    yield return new FactNeedsSharePointTestCase(
                        _diagnosticSink,
                        defaultMethodDisplay,
                        tc.TestMethod,
                        tc.TestMethodArguments
                    );
                }
            }
        }
    }
}
