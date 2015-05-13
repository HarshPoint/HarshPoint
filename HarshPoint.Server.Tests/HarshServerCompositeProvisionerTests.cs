using HarshPoint.Provisioning;
using HarshPoint.Server.Provisioning;
using Moq;
using Moq.Protected;
using System;
using System.Threading.Tasks;
using Xunit;

namespace HarshPoint.Server.Tests
{
    public class HarshServerCompositeProvisionerTests : IClassFixture<SharePointServerFixture>
    {
        public HarshServerCompositeProvisionerTests(SharePointServerFixture data)
        {
            ServerOM = data;
        }

        public SharePointServerFixture ServerOM
        {
            get;
            set;
        }

        [Fact]
        public async Task Calls_server_provision_in_correct_order()
        {
            var seq = String.Empty;

            var p1 = new Mock<HarshServerProvisioner>();
            var p2 = new Mock<HarshServerProvisioner>();

            p1.Protected()
                .Setup<Task>("OnProvisioningAsync")
                .Returns(HarshTask.Completed)
                .Callback(() => seq += "1");

            p2.Protected()
                .Setup<Task>("OnProvisioningAsync")
                .Returns(HarshTask.Completed)
                .Callback(() => seq += "2");

            var composite = new HarshServerProvisioner()
            {
                Children = { p1.Object, p2.Object }
            };

            await composite.ProvisionAsync(ServerOM.WebContext);

            Assert.Equal("12", seq);
        }

        [Fact]
        public async Task Calls_server_unprovision_in_correct_order()
        {
            var seq = String.Empty;

            var p1 = new Mock<HarshServerProvisioner>();
            var p2 = new Mock<HarshServerProvisioner>();

            p1.Protected()
                .Setup<Task>("OnUnprovisioningAsync")
                .Returns(HarshTask.Completed)
                .Callback(() => seq += "1");

            p2.Protected()
                .Setup<Task>("OnUnprovisioningAsync")
                .Returns(HarshTask.Completed)
                .Callback(() => seq += "2");

            var ctx = (HarshServerProvisionerContext)ServerOM.WebContext.Clone();
            ctx.MayDeleteUserData = true;

            var composite = new HarshServerProvisioner()
            {
                Children = { p1.Object, p2.Object }
            };
            await composite.UnprovisionAsync(ctx);

            Assert.Equal("21", seq);
        }

        [Fact]
        public async Task Adapts_client_provisioner_into_server()
        {
            var p = new Mock<HarshProvisioner>();

            p.Protected()
                .Setup<Task>("OnProvisioningAsync")
                .Returns(async () =>
                {
                    Assert.NotNull(p.Object.ClientContext);
                    Assert.NotNull(p.Object.Web);

                    p.Object.ClientContext.Load(p.Object.Web, w => w.Url);
                    await p.Object.ClientContext.ExecuteQueryAsync();

                    Assert.Equal(ServerOM.Web.Url, p.Object.Web.Url);
                });

            var composite = new HarshServerProvisioner()
            {
                Children = { p.Object }
            };
            
            await composite.ProvisionAsync(ServerOM.WebContext);
        }       
    }
}
