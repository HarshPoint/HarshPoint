using HarshPoint.Provisioning;
using Moq;
using Moq.Protected;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning
{
    public class HarshCompositeProvisionerTests : SharePointClientTest
    {
        public HarshCompositeProvisionerTests(SharePointClientFixture fixture, ITestOutputHelper output) : base(fixture, output)
        {
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

            var ctx = Context.AllowDeleteUserData();

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

            var ctx = Context.AllowDeleteUserData();

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
                    Assert.Equal(Web, p.Object.Web);
                });

            var composite = new HarshProvisioner()
            {
                Children = { p.Object }
            };
            await composite.ProvisionAsync(Context);
        }

        [Fact]
        public async Task Calls_child_Provision_with_modified_context_via_Modifier()
        {
            var composite = new ModifiesChildContextUsingModifier()
            {
                Children = { new ExpectsModifiedContext() }
            };
            await composite.ProvisionAsync(Context);
        }

        [Fact]
        public async Task Calls_child_Unprovision_with_modified_context_via_Modifier()
        {
            var composite = new ModifiesChildContextUsingModifier()
            {
                Children = { new ExpectsModifiedContext() }
            };
            await composite.UnprovisionAsync(Context);
        }
        
        private class ModifiesChildContextUsingModifier : HarshProvisioner
        {
            public ModifiesChildContextUsingModifier()
            {
                ModifyChildrenContextState(() => "1234");
            }

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
