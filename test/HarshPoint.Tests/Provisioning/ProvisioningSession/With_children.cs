using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using HarshPoint.Tests;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Session
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
            Assert.IsType<HarshChild2>(flattened[0]);
            Assert.IsType<HarshSubChild>(flattened[1]);
            Assert.IsType<HarshChild1>(flattened[2]);
            Assert.IsType<HarshProvisioner>(flattened[3]);
        }

        [Fact]
        public async Task Provisioning_inspector_notifications_are_in_correct_order()
        {
            var mockInspector = new Mock<IProvisioningSessionInspector>(MockBehavior.Strict);

            var seq = new Sequence();
            AddSessionStartingSequence(seq, mockInspector);

            AddProvisioningSequence<HarshProvisioner>(seq, mockInspector);
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
            AddProvisioningSequence<HarshProvisioner>(seq, mockInspector);

            AddSessionEndedSequence(seq, mockInspector);

            var context = Context.AddSessionInspector(mockInspector.Object);
            await Provisioner.UnprovisionAsync(context);

            seq.VerifyFinished();
        }
    }
}