using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using HarshPoint.Tests;
using Moq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Session
{
    public class With_skipped : ProvisioningSessionTest
    {
        public With_skipped(ITestOutputHelper output)
        : base(output)
        {
        }

        private HarshProvisioner Provisioner
            => new HarshProvisioner()
            {
                Children =
                {
                    new HarshProvisionerSkipped()
                    {
                        Children =
                        {
                            new HarshChild1()
                            {
                                Children =
                                {
                                    new HarshSubChild(),
                                },
                            },
                            new HarshChild2(),
                        },
                    },
                },
            };

        [Fact]
        public async Task Provisioning_inspector_notifications_are_in_correct_order()
        {
            var mockInspector = new Mock<IProvisioningSessionInspector>(MockBehavior.Strict);

            var seq = new Sequence();
            AddSessionStartingSequence(seq, mockInspector);

            AddProvisioningSequence<HarshProvisioner>(seq, mockInspector);
            AddProvisioningSequence<HarshProvisionerSkipped>(seq, mockInspector);
            AddProvisioningSequence<HarshChild1>(seq, mockInspector);
            AddProvisioningSequence<HarshSubChild>(seq, mockInspector);
            AddProvisioningSequence<HarshChild2>(seq, mockInspector);

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

            AddProvisioningSequence<HarshChild2>(seq, mockInspector);
            AddProvisioningSequence<HarshSubChild>(seq, mockInspector);
            AddProvisioningSequence<HarshChild1>(seq, mockInspector);
            AddProvisioningSkippedSequence<HarshProvisionerSkipped>(seq, mockInspector);
            AddProvisioningSequence<HarshProvisioner>(seq, mockInspector);

            AddSessionEndedSequence(seq, mockInspector);

            var context = Context.AddSessionInspector(mockInspector.Object);
            await Provisioner.UnprovisionAsync(context);

            seq.VerifyFinished();
        }
    }
}