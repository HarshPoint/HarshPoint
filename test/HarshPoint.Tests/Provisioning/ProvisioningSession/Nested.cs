using HarshPoint.ObjectModel;
using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using HarshPoint.Provisioning.Implementation;
using Moq;
using Moq.Sequences;

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

            using (Sequence.Create())
            {
                AddSessionStartingSequence(mockInspector);

                AddProvisioningSequence<HarshProvisioner>(mockInspector);
                AddProvisioningSequence<HarshChild1>(mockInspector);
                AddProvisioningSequence<HarshSubChild>(mockInspector);

                AddSessionEndedSequence(mockInspector);

                var context = Context.AddSessionInspector(mockInspector.Object);
                await Provisioner.ProvisionAsync(context);
            }
        }

        [Fact]
        public async Task Unprovisioning_inspector_notifications_are_in_correct_order()
        {
            var mockInspector = new Mock<IProvisioningSessionInspector>(MockBehavior.Strict);

            using (Sequence.Create())
            {
                AddSessionStartingSequence(mockInspector);

                AddProvisioningSequence<HarshProvisioner>(mockInspector);
                AddProvisioningSequence<HarshChild1>(mockInspector);
                AddProvisioningSequence<HarshSubChild>(mockInspector);

                AddSessionEndedSequence(mockInspector);

                var context = Context.AddSessionInspector(mockInspector.Object);
                await Provisioner.UnprovisionAsync(context);
            }
        }
    }
}