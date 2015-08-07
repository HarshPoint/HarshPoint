using HarshPoint.ObjectModel;
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
        public async Task Succeeds_string(String value)
        {
            var p = new SingleStringParam() { Param = value };
            await p.ProvisionAsync(Context);
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   \t  ")]
        public async Task Fails_string(String value)
        {
            var p = new SingleStringParam() { Param = value };

            await Assert.ThrowsAsync<ParameterValidationException>(
                () => p.ProvisionAsync(Context)
            );
        }

        [Fact]
        public async Task Succeeds_non_null_nullable()
        {
            var p = new SingleNullableInt32Param()
            {
                Param = 42
            };

            await p.ProvisionAsync(Context);
        }

        [Fact]
        public async Task Fails_null_nullable()
        {
            var p = new SingleNullableInt32Param()
            {
                Param = null
            };

            await Assert.ThrowsAsync<ParameterValidationException>(
                () => p.ProvisionAsync(Context)
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
