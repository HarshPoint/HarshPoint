using HarshPoint.Provisioning;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning
{
    public class MandatoryParameters : SharePointClientTest
    {
        public MandatoryParameters(SharePointClientFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }

        [Theory]
        [InlineData("test")]
        [InlineData("   \t  ")]
        public async Task Succeeds_string(String value)
        {
            var p = new SingleStringParam() { Param = value };
            await p.ProvisionAsync(Fixture.Context);
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task Fails_string(String value)
        {
            var p = new SingleStringParam() { Param = value };

            await Assert.ThrowsAsync<ParameterValidationException>(
                () => p.ProvisionAsync(Fixture.Context)
            );
        }

        [Fact]
        public async Task Succeeds_non_null_nullable()
        {
            var p = new SingleNullableInt32Param()
            {
                Param = 42
            };

            await p.ProvisionAsync(Fixture.Context);
        }

        [Fact]
        public async Task Fails_null_nullable()
        {
            var p = new SingleNullableInt32Param()
            {
                Param = null
            };

            await Assert.ThrowsAsync<ParameterValidationException>(
                () => p.ProvisionAsync(Fixture.Context)
            );
        }

        private sealed class SingleStringParam : HarshProvisioner
        {
            [Parameter(Mandatory = true)]
            public String Param
            {
                get;
                set;
            }
        }

        private sealed class SingleNullableInt32Param : HarshProvisioner
        {
            [Parameter(Mandatory = true)]
            public Int32? Param
            {
                get;
                set;
            }
        }
    }
}
