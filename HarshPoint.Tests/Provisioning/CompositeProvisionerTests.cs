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
    }
}
