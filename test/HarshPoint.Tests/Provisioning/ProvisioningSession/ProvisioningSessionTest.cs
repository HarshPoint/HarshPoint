using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using HarshPoint.Tests;
using Moq;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Session
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
                => base.OnUnprovisioningAsync();
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

        protected void AddProvisioningSequence<TProvisioner>(
            Sequence seq,
            Mock<IProvisioningSessionInspector> mockInspector
        )
            where TProvisioner : HarshProvisionerBase
        {
            mockInspector
                .SetupInSequence(seq, x => x.OnProvisioningStarting(
                    It.IsAny<HarshProvisionerContext>(),
                    It.IsAny<TProvisioner>()
                ));
            mockInspector
                .SetupInSequence(seq, x => x.OnProvisioningEnded(
                    It.IsAny<HarshProvisionerContext>(),
                    It.IsAny<TProvisioner>()
                ));
        }

        protected void AddProvisioningSkippedSequence<TProvisioner>(
            Sequence seq,
            Mock<IProvisioningSessionInspector> mockInspector
        )
            where TProvisioner : HarshProvisionerBase
        {
            mockInspector
                .SetupInSequence(seq, x => x.OnProvisioningSkipped(
                    It.IsAny<HarshProvisionerContext>(),
                    It.IsAny<TProvisioner>()
                ));
        }

        protected void AddSessionStartingSequence(
            Sequence seq,
            Mock<IProvisioningSessionInspector> mockInspector
        )
        {
            mockInspector
                .SetupInSequence(seq, x => x.OnSessionStarting(
                    It.IsAny<HarshProvisionerContext>()
                ));
        }

        protected void AddSessionEndedSequence(
            Sequence seq,
            Mock<IProvisioningSessionInspector> mockInspector
        )
        {
            mockInspector
                .SetupInSequence(seq, x => x.OnSessionEnded(
                    It.IsAny<HarshProvisionerContext>()
                ));
        }
    }
}