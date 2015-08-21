using HarshPoint;
using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator;
using HarshPoint.ShellployGenerator.Builders;
using HarshPoint.Tests;
using System;
using Xunit;
using Xunit.Abstractions;

namespace CommandBuilding
{
    public class Fails_when : SeriloggedTest
    {
        public Fails_when(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Has_InputObject_parameter()
        {
            Assert.Throws<ArgumentException>(() =>
                new NewObjectCommandBuilder<InputObjectProvisioner>()
            );
        }


        [Fact]
        public void Sets_parameter_default_then_fixed()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var builder = new NewObjectCommandBuilder<Provisioner>();

                builder.Parameter(x => x.Param)
                    .SetDefaultValue("42")
                    .SetFixedValue("4242");
            });
        }


        [Fact]
        public void Sets_parameter_fixed_then_default()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var builder = new NewObjectCommandBuilder<Provisioner>();

                builder.Parameter(x => x.Param)
                    .SetFixedValue("4242")
                    .SetDefaultValue("42");
            });
        }

        private sealed class InputObjectProvisioner : HarshProvisioner
        {
            [Parameter]
            public String InputObject { get; set; }
        }

        private sealed class Provisioner : HarshProvisioner
        {
            [Parameter]
            public String Param { get; set; }
        }
    }
}
