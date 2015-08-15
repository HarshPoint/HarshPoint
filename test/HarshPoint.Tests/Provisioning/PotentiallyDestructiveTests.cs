using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning
{
    public class PotentiallyDestructiveTests : SharePointClientTest
    {
        public PotentiallyDestructiveTests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        public async Task Destructive_Unprovision_not_called_by_default()
        {
            var destructive = new DestructiveUnprovision();
            await destructive.UnprovisionAsync(Context);
        }

        [Fact]
        public async Task Destructive_Unprovision_not_called_when_MayDeleteUserData()
        {
            var ctx = Context.AllowDeleteUserData();

            var destructive = new DestructiveUnprovision();
            await Assert.ThrowsAsync<InvalidOperationException>(() => destructive.UnprovisionAsync(ctx));
        }

        [Fact]
        public async Task Safe_Unprovision_called_by_default()
        {
            var safe = new NeverDeletesUnprovision();
            await Assert.ThrowsAsync<InvalidOperationException>(() => safe.UnprovisionAsync(Context));
        }

        [Fact]
        public async Task Safe_Unprovision_called_when_MayDeleteUserData()
        {
            var ctx = Context.AllowDeleteUserData();
            var safe = new NeverDeletesUnprovision();
            await Assert.ThrowsAsync<InvalidOperationException>(() => safe.UnprovisionAsync(ctx));
        }

        [Fact]
        public void Destructive_Unprovision_with_safe_base_type_considered_destructive()
        {
            var metadata = new HarshProvisionerMetadata(typeof(DestructiveUnprovisionSafeBase));
            Assert.True(metadata.UnprovisionDeletesUserData);
        }

        [Fact]
        public async Task Unprovisioner_with_MayDeleteUserData_runs_with_context_MayDeleteUserData_false()
        {
            var ctx = Context.AllowDeleteUserData();

            var prov = new DestructiveUnprovision();
            prov.MayDeleteUserData = true;

            await Assert.ThrowsAsync<InvalidOperationException>(() => prov.UnprovisionAsync(ctx));
        }

        [Fact]
        public async Task Unprovisioner_without_MayDeleteUserData_runs_with_context_MayDeleteUserData_true()
        {
            var ctx = Context.AllowDeleteUserData();

            var prov = new DestructiveUnprovision();
            prov.MayDeleteUserData = false;

            await Assert.ThrowsAsync<InvalidOperationException>(() => prov.UnprovisionAsync(ctx));
        }

        private class DestructiveUnprovision : HarshProvisioner
        {
            protected override Task OnUnprovisioningAsync()
            {
                throw new InvalidOperationException("In an unsafe OnUnprovisioningAsync.");
            }
        }

        private class NeverDeletesUnprovision : HarshProvisioner
        {
            [NeverDeletesUserData]
            protected override Task OnUnprovisioningAsync()
            {
                throw new InvalidOperationException("In a safe OnUnprovisioningAsync.");
            }
        }

        private class DestructiveUnprovisionSafeBase : NeverDeletesUnprovision
        {
            protected override Task OnUnprovisioningAsync() => base.OnUnprovisioningAsync();
        }
    }
}
