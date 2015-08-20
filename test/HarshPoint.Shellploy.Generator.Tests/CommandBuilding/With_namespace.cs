using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator;
using HarshPoint.ShellployGenerator.Builders;
using HarshPoint.Tests;
using Xunit;
using Xunit.Abstractions;

namespace CommandBuilding
{
    public class With_namespace : SeriloggedTest
    {
        public With_namespace(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Has_namespace()
        {
            var ns = "MyNamespace";

            var builder = new CommandBuilder<EmptyProvisioner>();
            builder.Namespace = ns;

            var command = builder.ToCommand();
            Assert.Equal(ns, command.Namespace);
        }
        private class EmptyProvisioner : HarshProvisioner
        {
        }
    }
}
