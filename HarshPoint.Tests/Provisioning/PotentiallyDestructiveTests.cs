using HarshPoint.Provisioning;
using System;
using System.Threading.Tasks;
using Xunit;

namespace HarshPoint.Tests.Provisioning
{
    public class PotentiallyDestructiveTests : IClassFixture<SharePointClientFixture>
    {
        public PotentiallyDestructiveTests(SharePointClientFixture data)
        {
            ClientOM = data;
        }

        public SharePointClientFixture ClientOM { get; private set; }

        [Fact]
        public async Task Destructive_Unprovision_not_called_by_default()
        {
            var destructive = new DestructiveUnprovision();
            await destructive.UnprovisionAsync(ClientOM.Context);
        }

        [Fact]
        public void Destructive_Unprovision_not_called_when_MayDeleteUserData()
        {
            var ctx = (HarshProvisionerContext)ClientOM.Context.Clone();
            ctx.MayDeleteUserData = true;

            var destructive = new DestructiveUnprovision();
            Assert.ThrowsAsync<InvalidOperationException>(async () => await destructive.UnprovisionAsync(ctx));
        }

        [Fact]
        public void Safe_Unprovision_called_by_default()
        {
            var safe = new NeverDeletesUnprovision();
            Assert.ThrowsAsync<InvalidOperationException>(async () => await safe.UnprovisionAsync(ClientOM.Context));
        }

        [Fact]
        public void Safe_Unprovision_called_when_MayDeleteUserData()
        {
            var ctx = (HarshProvisionerContext)ClientOM.Context.Clone();
            ctx.MayDeleteUserData = true;

            var safe = new NeverDeletesUnprovision();
            Assert.ThrowsAsync<InvalidOperationException>(async () => await safe.UnprovisionAsync(ctx));
        }

        [Fact]
        public void Destructive_Unprovision_with_safe_base_type_considered_destructive()
        {
            var metadata = new HarshProvisionerMetadata(typeof(DestructiveUnprovisionSafeBase));
            Assert.True(metadata.UnprovisionDeletesUserData);
        }

        private class DestructiveUnprovision : HarshProvisioner
        {
            protected override Task OnUnprovisioningAsync()
            {
                throw new InvalidOperationException("should not have been called");
            }
        }

        private class NeverDeletesUnprovision : HarshProvisioner
        {
            [NeverDeletesUserData]
            protected override Task OnUnprovisioningAsync()
            {
                throw new InvalidOperationException("should not have been called");
            }
        }

        private class DestructiveUnprovisionSafeBase : NeverDeletesUnprovision
        {
            protected override Task OnUnprovisioningAsync()
            {
                return base.OnUnprovisioningAsync();
            }
        }
    }
}
