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
    public class With_children : ProvisioningSessionTest
    {
        public With_children(ITestOutputHelper output)
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
                    new HarshChild2(),
                },
            };

        [Fact]
        public void GetChildrenSorted_provisioning_returns_correct_order()
        {
            var children = Provisioner
                .GetChildrenSorted(HarshProvisionerAction.Provision)
                .ToArray();
            Assert.Equal(2, children.Length);
            Assert.IsType<HarshChild1>(children[0]);
            Assert.IsType<HarshChild2>(children[1]);
        }

        [Fact]
        public void GetChildrenSorted_unprovisioning_returns_correct_order()
        {
            var children = Provisioner
                .GetChildrenSorted(HarshProvisionerAction.Unprovision)
                .ToArray();
            Assert.Equal(2, children.Length);
            Assert.IsType<HarshChild2>(children[0]);
            Assert.IsType<HarshChild1>(children[1]);
        }

        [Fact]
        public void GetFlattenedTree_provisioning_returns_correct_order()
        {
            var provisioner = Provisioner;
            var session = new HarshPoint.Provisioning.Implementation.ProvisioningSession(
                provisioner,
                HarshProvisionerAction.Provision
            );
            var flattened = session
                .GetFlattenedTree(provisioner)
                .ToArray();

            Assert.Equal(4, flattened.Length);
            Assert.IsType<HarshProvisioner>(flattened[0]);
            Assert.IsType<HarshChild1>(flattened[1]);
            Assert.IsType<HarshSubChild>(flattened[2]);
            Assert.IsType<HarshChild2>(flattened[3]);
        }

        [Fact]
        public void GetFlattenedTree_unprovisioning_returns_correct_order()
        {
            var provisioner = Provisioner;
            var session = new HarshPoint.Provisioning.Implementation.ProvisioningSession(
                provisioner,
                HarshProvisionerAction.Unprovision
            );
            var flattened = session
                .GetFlattenedTree(provisioner)
                .ToArray();

            Assert.Equal(4, flattened.Length);
            Assert.IsType<HarshProvisioner>(flattened[0]);
            Assert.IsType<HarshChild2>(flattened[1]);
            Assert.IsType<HarshChild1>(flattened[2]);
            Assert.IsType<HarshSubChild>(flattened[3]);
        }

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
                AddProvisioningSequence<HarshChild2>(mockInspector);

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
                AddProvisioningSequence<HarshChild2>(mockInspector);
                AddProvisioningSequence<HarshChild1>(mockInspector);
                AddProvisioningSequence<HarshSubChild>(mockInspector);

                AddSessionEndedSequence(mockInspector);

                var context = Context.AddSessionInspector(mockInspector.Object);
                await Provisioner.UnprovisionAsync(context);
            }
        }
    }
}