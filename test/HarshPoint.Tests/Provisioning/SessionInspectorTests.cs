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

namespace HarshPoint.Tests.Provisioning
{
    public class Session_inspector : SharePointClientTest
    {
        public Session_inspector(ITestOutputHelper output)
        : base(output)
        {
        }

        private HarshProvisioner ProvisionerNested
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

        private HarshProvisioner ProvisionerWithChildren
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

        private HarshProvisioner ProvisionerWithoutUnprovisioning
            => new HarshProvisioner()
            {
                Children =
                {
                    new HarshProvisionerWithoutUnprovisioning()
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
        public async Task Provisioning_notifications_are_in_correct_order_nested()
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
                await ProvisionerNested.ProvisionAsync(context);
            }
        }

        [Fact]
        public async Task Unprovisioning_notifications_are_in_correct_order_nested()
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
                await ProvisionerNested.UnprovisionAsync(context);
            }
        }

        [Fact]
        public async Task Provisioning_notifications_are_in_correct_order_with_children()
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
                await ProvisionerWithChildren.ProvisionAsync(context);
            }
        }

        [Fact]
        public async Task Unprovisioning_notifications_are_in_correct_order_with_children()
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
                await ProvisionerWithChildren.UnprovisionAsync(context);
            }
        }

        [Fact]
        public async Task Unprovisioning_notifications_are_in_correct_order_when_skipped()
        {
            var mockInspector = new Mock<IProvisioningSessionInspector>(MockBehavior.Strict);

            using (Sequence.Create())
            {
                AddSessionStartingSequence(mockInspector);

                AddProvisioningSequence<HarshProvisioner>(mockInspector);
                AddProvisioningSkippedSequence<HarshProvisionerWithoutUnprovisioning>(mockInspector);
                AddProvisioningSkippedSequence<HarshChild2>(mockInspector);
                AddProvisioningSkippedSequence<HarshChild1>(mockInspector);
                AddProvisioningSkippedSequence<HarshSubChild>(mockInspector);

                AddSessionEndedSequence(mockInspector);

                var context = Context.AddSessionInspector(mockInspector.Object);
                await ProvisionerWithoutUnprovisioning.UnprovisionAsync(context);
            }
        }

        private class HarshProvisionerWithoutUnprovisioning : HarshProvisioner
        {
            protected override Task OnUnprovisioningAsync()
            {
                return base.OnUnprovisioningAsync();
            }
        }

        private class HarshChild1 : HarshProvisioner
        {
        }

        private class HarshChild2 : HarshProvisioner
        {
        }

        private class HarshSubChild : HarshProvisioner
        {
        }

        private void AddProvisioningSequence<TProvisioner>(Mock<IProvisioningSessionInspector> mockInspector)
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

        private void AddProvisioningSkippedSequence<TProvisioner>(Mock<IProvisioningSessionInspector> mockInspector)
            where TProvisioner : HarshProvisionerBase
        {
            mockInspector.Setup(x => x.OnProvisioningSkipped(
                It.IsAny<HarshProvisionerContext>(),
                It.IsAny<TProvisioner>()
            )).InSequence();
        }

        private void AddSessionStartingSequence(Mock<IProvisioningSessionInspector> mockInspector)
        {
            mockInspector.Setup(x => x.OnSessionStarting(
                It.IsAny<HarshProvisionerContext>()
            )).InSequence();
        }

        private void AddSessionEndedSequence(Mock<IProvisioningSessionInspector> mockInspector)
        {
            mockInspector.Setup(x => x.OnSessionEnded(
                It.IsAny<HarshProvisionerContext>()
            )).InSequence();
        }
    }
}