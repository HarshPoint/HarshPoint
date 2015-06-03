using HarshPoint.Provisioning;
using Moq;
using Moq.Protected;
using System;
using System.Threading.Tasks;
using Xunit;

namespace HarshPoint.Tests.Provisioning
{
    public class HarshCompositeProvisionerTests : IClassFixture<SharePointClientFixture>
    {
        public HarshCompositeProvisionerTests(SharePointClientFixture fixture)
        {
            ClientOM = fixture;
        }

        public SharePointClientFixture ClientOM
        {
            get;
            set;
        }

        [Fact]
        public async Task Calls_children_provision_in_correct_order()
        {
            var seq = String.Empty;

            var p1 = new Mock<HarshProvisioner>();
            var p2 = new Mock<HarshProvisioner>();

            p1.Protected()
                .Setup<Task>("OnProvisioningAsync")
                .Returns(HarshTask.Completed)
                .Callback(() => seq += "1");

            p2.Protected()
                .Setup<Task>("OnProvisioningAsync")
                .Returns(HarshTask.Completed)
                .Callback(() => seq += "2");

            var ctx = ClientOM.Context.AllowDeleteUserData();

            var composite = new HarshProvisioner()
            {
                Children = { p1.Object, p2.Object }
            };
            await composite.ProvisionAsync(ctx);

            Assert.Equal("12", seq);
        }

        [Fact]
        public async Task Calls_children_unprovision_in_correct_order()
        {
            var seq = String.Empty;

            var p1 = new Mock<HarshProvisioner>();
            var p2 = new Mock<HarshProvisioner>();

            p1.Protected()
                .Setup<Task>("OnUnprovisioningAsync")
                .Returns(HarshTask.Completed)
                .Callback(() => seq += "1");

            p2.Protected()
                .Setup<Task>("OnUnprovisioningAsync")
                .Returns(HarshTask.Completed)
                .Callback(() => seq += "2");

            var ctx = ClientOM.Context.AllowDeleteUserData();

            var composite = new HarshProvisioner()
            {
                Children = { p1.Object, p2.Object }
            };
            await composite.UnprovisionAsync(ctx);

            Assert.Equal("21", seq);
        }

        [Fact]
        public async Task Assigns_context_to_children()
        {
            var p = new Mock<HarshProvisioner>();
            p.Protected()
                .Setup<Task>("OnProvisioningAsync")
                .Returns(HarshTask.Completed)
                .Callback(() =>
                {
                    Assert.Equal(ClientOM.Web, p.Object.Web);
                });

            var composite = new HarshProvisioner()
            {
                Children = { p.Object }
            };
            await composite.ProvisionAsync(ClientOM.Context);
        }

        [Fact]
        public async Task Calls_child_Provision_with_modified_context()
        {
            var composite = new ModifiesChildContext()
            {
                Children = { new ExpectsModifiedContext() }
            };
            await composite.ProvisionAsync(ClientOM.Context);
        }

        [Fact]
        public async Task Calls_child_Unprovision_with_modified_context()
        {
            var composite = new ModifiesChildContext()
            {
                Children = { new ExpectsModifiedContext() }
            };
            await composite.UnprovisionAsync(ClientOM.Context);
        }

        [Fact]
        public async Task Calls_child_Provision_with_modified_context_via_CreateChildrenContext()
        {
            var composite = new ModifiesChildContextUsingCreateChildrenContext()
            {
                Children = { new ExpectsModifiedContext() }
            };
            await composite.ProvisionAsync(ClientOM.Context);
        }

        [Fact]
        public async Task Calls_child_Unprovision_with_modified_context_via_CreateChildrenContext()
        {
            var composite = new ModifiesChildContextUsingCreateChildrenContext()
            {
                Children = { new ExpectsModifiedContext() }
            };
            await composite.UnprovisionAsync(ClientOM.Context);
        }

        private class ModifiesChildContext  : HarshProvisioner
        {
            protected override Task OnProvisioningAsync()
            {
                Assert.Empty(Context.StateStack);
                return ProvisionChildrenAsync(Context.PushState("1234"));
            }

            [NeverDeletesUserData]
            protected override Task OnUnprovisioningAsync()
            {
                Assert.Empty(Context.StateStack);
                return UnprovisionChildrenAsync(Context.PushState("1234"));
            }
        }

        private class ModifiesChildContextUsingCreateChildrenContext : HarshProvisioner
        {
            protected override Task OnProvisioningAsync()
            {
                Assert.Empty(Context.StateStack);
                return base.OnProvisioningAsync();
            }

            [NeverDeletesUserData]
            protected override Task OnUnprovisioningAsync()
            {
                Assert.Empty(Context.StateStack);
                return base.OnUnprovisioningAsync();
            }

            protected override HarshProvisionerContext CreateChildrenContext()
            {
                return Context.PushState("1234");
            }
        }

        private class ExpectsModifiedContext : HarshProvisioner
        {
            protected override Task OnProvisioningAsync()
            {
                Assert.Single(Context.StateStack, "1234");
                return base.OnProvisioningAsync();
            }

            [NeverDeletesUserData]
            protected override Task OnUnprovisioningAsync()
            {
                Assert.Single(Context.StateStack, "1234");
                return base.OnUnprovisioningAsync();
            }
        }
    }
}
