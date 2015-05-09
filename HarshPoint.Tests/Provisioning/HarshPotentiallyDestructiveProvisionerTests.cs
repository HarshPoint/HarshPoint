using System;
using HarshPoint.Provisioning;
using Moq;
using Moq.Protected;
using Xunit;

namespace HarshPoint.Tests.Provisioning
{
    public class HarshPotentiallyDestructiveProvisionerTests : IUseFixture<SharePointClientFixture>
    {
        public SharePointClientFixture ClientOM { get; private set; }

        [Fact]
        public void Destructive_Unprovision_not_called_by_default()
        {
            var destructive = new DestructiveUnprovision();
            Assert.DoesNotThrow(() => destructive.Unprovision(ClientOM.Context));
        }

        [Fact]
        public void Destructive_Unprovision_not_called_when_MayDeleteUserData()
        {
            var ctx = (HarshProvisionerContext)ClientOM.Context.Clone();
            ctx.MayDeleteUserData = true;

            var destructive = new DestructiveUnprovision();
            Assert.Throws<InvalidOperationException>(() => destructive.Unprovision(ctx));
        }

        [Fact]
        public void Safe_Unprovision_called_by_default()
        {
            var safe = new NeverDeletesUnprovision();
            Assert.Throws<InvalidOperationException>(() => safe.Unprovision(ClientOM.Context));
        }

        [Fact]
        public void Safe_Unprovision_called_when_MayDeleteUserData()
        {
            var ctx = (HarshProvisionerContext)ClientOM.Context.Clone();
            ctx.MayDeleteUserData = true;

            var safe = new NeverDeletesUnprovision();
            Assert.Throws<InvalidOperationException>(() => safe.Unprovision(ctx));
        }

        [Fact]
        public void Destructive_Unprovision_with_safe_base_type_considered_destructive()
        {
            var metadata = new HarshProvisionerMetadata(typeof(DestructiveUnprovisionSafeBase));
            Assert.True(metadata.UnprovisionDeletesUserData);
        }

        public void SetFixture(SharePointClientFixture data)
        {
            ClientOM = data;
        }

        private class DestructiveUnprovision : HarshProvisioner
        {
            protected override void OnUnprovisioning()
            {
                throw new InvalidOperationException("should not have been called");
            }
        }

        private class NeverDeletesUnprovision : HarshProvisioner
        {
            [NeverDeletesUserData]
            protected override void OnUnprovisioning()
            {
                throw new InvalidOperationException("should not have been called");
            }
        }

        private class DestructiveUnprovisionSafeBase : NeverDeletesUnprovision
        {
            protected override void OnUnprovisioning()
            {
                base.OnUnprovisioning();
            }
        }
    }
}
