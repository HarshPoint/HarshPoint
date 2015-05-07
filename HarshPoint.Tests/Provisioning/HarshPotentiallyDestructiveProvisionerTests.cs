using HarshPoint.Provisioning;
using Moq;
using Moq.Protected;
using Xunit;

namespace HarshPoint.Tests.Provisioning
{
    public class HarshPotentiallyDestructiveProvisionerTests
    {
        [Fact(Skip = "to be refactored")]
        public void Unprovision_not_called_when_DeleteUserData_false()
        {
            var mock = new Mock<HarshPotentiallyDestructiveProvisioner>(MockBehavior.Strict);
            mock.Protected().Setup("Initialize");
            mock.Protected().Setup("Complete");

            var obj = mock.Object;
            obj.DeleteUserDataWhenUnprovisioning = false;
            //obj.Unprovision();

            mock.Verify();
        }

        [Fact(Skip = "to be refactored")]
        public void Unprovision_called_when_DeleteUserData_true()
        {
            var mock = new Mock<HarshPotentiallyDestructiveProvisioner>();
            mock.Protected().Setup("OnUnprovisioningMayDeleteUserData").Verifiable();

            var obj = mock.Object;
            obj.DeleteUserDataWhenUnprovisioning = true;
            //obj.Unprovision();

            mock.Verify();
        }
    }
}
