using Xunit;
using Xunit.Sdk;

namespace HarshPoint.Tests
{
    [XunitTestCaseDiscoverer(
        "HarshPoint.Tests.TheoryNeedsSharePointDiscoverer", 
        "HarshPoint.Tests"
    )]
    public sealed class TheoryNeedsSharePointAttribute : TheoryAttribute
    {
    }
}
