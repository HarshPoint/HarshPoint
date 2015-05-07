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

        public void Unprovision_not_called_when_DeleteUserData_false()
        {
            var mock = new Mock<HarshProvisioner>(MockBehavior.Strict);
            mock.Protected().Setup("Initialize");
            mock.Protected().Setup("Complete");

            var obj = mock.Object;
            obj.DeleteUserDataWhenUnprovisioning = false;
            obj.Unprovision(ClientOM.Context);

            mock.Verify();
        }

        public void Unprovision_called_when_DeleteUserData_true()
        {
            var mock = new Mock<HarshProvisioner>();
            mock.Protected().Setup("OnUnprovisioningMayDeleteUserData").Verifiable();

            var obj = mock.Object;
            obj.DeleteUserDataWhenUnprovisioning = true;
            obj.Unprovision(ClientOM.Context);

            mock.Verify();
        }

        public void SetFixture(SharePointClientFixture data)
        {
            ClientOM = data;
        }
    }
}
