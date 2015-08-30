using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using HarshPoint.Tests;
using Moq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ProvisioningSession
{
    public class Single : ProvisioningSessionTest
    {
        public Single(ITestOutputHelper output)
        : base(output)
        {
        }

        private HarshProvisioner Provisioner
            => new HarshProvisioner()
            {
            };

        [Fact]
        public async Task Provisioning_inspector_notifications_are_in_correct_order()
        {
            var mockInspector = new Mock<IProvisioningSessionInspector>(MockBehavior.Strict);
            var seq = new Sequence();
            AddSessionStartingSequence(seq, mockInspector);

            AddProvisioningSequence<HarshProvisioner>(seq, mockInspector);

            AddSessionEndedSequence(seq, mockInspector);

            var context = Context.AddSessionInspector(mockInspector.Object);
            await Provisioner.ProvisionAsync(context);

            seq.VerifyFinished();
        }

        [Fact]
        public async Task Unprovisioning_inspector_notifications_are_in_correct_order()
        {
            var mockInspector = new Mock<IProvisioningSessionInspector>(MockBehavior.Strict);

            var seq = new Sequence();
            AddSessionStartingSequence(seq, mockInspector);

            AddProvisioningSequence<HarshProvisioner>(seq, mockInspector);

            AddSessionEndedSequence(seq, mockInspector);

            var context = Context.AddSessionInspector(mockInspector.Object);
            await Provisioner.UnprovisionAsync(context);

            seq.VerifyFinished();
        }
    }
}