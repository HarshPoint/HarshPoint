using HarshPoint.ObjectModel;
using HarshPoint.Provisioning;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning
{
    public class MandatoryWhenCreating : SharePointClientTest
    {
        public MandatoryWhenCreating(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public Task Doesnt_throw_when_not_creating_and_null()
        {
            var prov = new TestProvisioner()
            {
                MandatoryWhenCreating = null,
                IsCreating = false
            };

            return prov.ProvisionAsync(Context);
        }

        [Fact]
        public Task Doesnt_throw_when_not_creating_and_non_null()
        {
            var prov = new TestProvisioner()
            {
                MandatoryWhenCreating = "not null",
                IsCreating = false
            };

            return prov.ProvisionAsync(Context);
        }

        [Fact]
        public Task Throws_when_creating_and_null()
        {
            var prov = new TestProvisioner()
            {
                MandatoryWhenCreating = null,
                IsCreating = true
            };

            return Assert.ThrowsAsync<ParameterValidationException>(
                () => prov.ProvisionAsync(Context)
            );
        }

        [Fact]
        public Task Doesnt_throw_when_creating_and_non_null()
        {
            var prov = new TestProvisioner()
            {
                MandatoryWhenCreating = "not null",
                IsCreating = true
            };

            return prov.ProvisionAsync(Context);
        }

        private sealed class TestProvisioner : HarshProvisioner
        {
            [Parameter]
            [MandatoryWhenCreating]
            public String MandatoryWhenCreating { get; set; }

            public Boolean IsCreating { get; set; }

            protected override Task OnProvisioningAsync()
            {
                if (IsCreating)
                {
                    ValidateMandatoryWhenCreatingParameters();
                }

                return base.OnProvisioningAsync();
            }
        }
    }
}
