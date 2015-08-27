using HarshPoint.Provisioning;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using HarshPoint.Provisioning.Implementation;
using Moq;
using Moq.Sequences;
using HarshPoint.Tests;

namespace ProvisioningSession
{
    public abstract class ProvisioningSessionTest : SharePointClientTest
    {
        public ProvisioningSessionTest(ITestOutputHelper output)
        : base(output)
        {
        }

        protected class HarshProvisionerSkipped : HarshProvisioner
        {
            // Missing HarshPoint.Provisioning.NeverDeletesUserData
            protected override Task OnUnprovisioningAsync()
            {
                return base.OnUnprovisioningAsync();
            }
        }

        protected class HarshChild1 : HarshProvisioner
        {
        }

        protected class HarshChild2 : HarshProvisioner
        {
        }

        protected class HarshSubChild : HarshProvisioner
        {
        }

        protected void AddProvisioningSequence<TProvisioner>(Mock<IProvisioningSessionInspector> mockInspector)
        where TProvisioner : HarshProvisionerBase
        {
            mockInspector.Setup(x => x.OnProvisioningStarting(
                It.IsAny<HarshProvisionerContext>(),
                It.IsAny<TProvisioner>()
            )).InSequence();
            mockInspector.Setup(x => x.OnProvisioningEnded(
                It.IsAny<HarshProvisionerContext>(),
                It.IsAny<TProvisioner>()
            )).InSequence();
        }

        protected void AddProvisioningSkippedSequence<TProvisioner>(Mock<IProvisioningSessionInspector> mockInspector)
            where TProvisioner : HarshProvisionerBase
        {
            mockInspector.Setup(x => x.OnProvisioningSkipped(
                It.IsAny<HarshProvisionerContext>(),
                It.IsAny<TProvisioner>()
            )).InSequence();
        }

        protected void AddSessionStartingSequence(Mock<IProvisioningSessionInspector> mockInspector)
        {
            mockInspector.Setup(x => x.OnSessionStarting(
                It.IsAny<HarshProvisionerContext>()
            )).InSequence();
        }

        protected void AddSessionEndedSequence(Mock<IProvisioningSessionInspector> mockInspector)
        {
            mockInspector.Setup(x => x.OnSessionEnded(
                It.IsAny<HarshProvisionerContext>()
            )).InSequence();
        }
    }
}