using Xunit.Sdk;
using Xunit;

namespace HarshPoint.Server.Tests
{
    [XunitTestCaseDiscoverer("HarshPoint.Server.Tests.FarmDependentTestCaseDiscoverer", "HarshPoint.Server.Tests")]
    public sealed class FactNeedsFarmAttribute : FactAttribute { }
}
