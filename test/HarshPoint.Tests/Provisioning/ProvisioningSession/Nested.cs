using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using HarshPoint.Tests;
using Moq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ProvisioningSession
{
    public class Nested : ProvisioningSessionTest
    {
        public Nested(ITestOutputHelper output)
        : base(output)
        {
        }

        private HarshProvisioner Provisioner
            => new HarshProvisioner()
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
                },
            };

        [Fact]
        public async Task Provisioning_inspector_notifications_are_in_correct_order()
        {
            var mockInspector = new Mock<IProvisioningSessionInspector>(MockBehavior.Strict);

            using (var seq = new Sequence())
            {
                AddSessionStartingSequence(seq, mockInspector);

                AddProvisioningSequence<HarshProvisioner>(seq, mockInspector);
                AddProvisioningSequence<HarshChild1>(seq, mockInspector);
                AddProvisioningSequence<HarshSubChild>(seq, mockInspector);

                AddSessionEndedSequence(seq, mockInspector);

                var context = Context.AddSessionInspector(mockInspector.Object);
                await Provisioner.ProvisionAsync(context);
            }
        }

        [Fact]
        public async Task Unprovisioning_inspector_notifications_are_in_correct_order()
        {
            var mockInspector = new Mock<IProvisioningSessionInspector>(MockBehavior.Strict);

            using (var seq = new Sequence())
            {
                AddSessionStartingSequence(seq, mockInspector);

                AddProvisioningSequence<HarshProvisioner>(seq, mockInspector);
                AddProvisioningSequence<HarshChild1>(seq, mockInspector);
                AddProvisioningSequence<HarshSubChild>(seq, mockInspector);

                AddSessionEndedSequence(seq, mockInspector);

                var context = Context.AddSessionInspector(mockInspector.Object);
                await Provisioner.UnprovisionAsync(context);
            }
        }
    }
}