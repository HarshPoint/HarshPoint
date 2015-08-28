using Xunit;
using Xunit.Sdk;

namespace HarshPoint.Tests
{
    [XunitTestCaseDiscoverer(
        "HarshPoint.Tests.FactNeedsSharePointDiscoverer", 
        "HarshPoint.Tests"
    )]
    public sealed class TheoryNeedsSharePointAttribute : TheoryAttribute
    {
    }
}
