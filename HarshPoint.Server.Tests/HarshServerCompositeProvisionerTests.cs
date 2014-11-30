using HarshPoint.Provisioning;
using HarshPoint.Server.Provisioning;
using Moq;
using Moq.Protected;
using System;
using Xunit;

namespace HarshPoint.Server.Tests
{
    public class HarshServerCompositeProvisionerTests : IUseFixture<SharePointServerFixture>
    {
        [Fact]
        public void Calls_server_provision_in_correct_order()
        {
            var seq = String.Empty;

            var p1 = new Mock<HarshServerProvisioner>();
            var p2 = new Mock<HarshServerProvisioner>();

            p1.Protected().Setup("OnProvisioning").Callback(() => seq += "1");
            p2.Protected().Setup("OnProvisioning").Callback(() => seq += "2");

            var composite = new HarshServerCompositeProvisioner()
             {
                 Provisioners = { p1.Object, p2.Object }
             };

            composite.Provision();

            Assert.Equal("12", seq);
        }

        [Fact]
        public void Calls_server_unprovision_in_correct_order()
        {
            var seq = String.Empty;

            var p1 = new Mock<HarshServerProvisioner>();
            var p2 = new Mock<HarshServerProvisioner>();

            p1.Protected().Setup("OnUnprovisioning").Callback(() => seq += "1");
            p2.Protected().Setup("OnUnprovisioning").Callback(() => seq += "2");

            var composite = new HarshServerCompositeProvisioner()
             {
                 Provisioners = { p1.Object, p2.Object }
             };

            composite.Unprovision();

            Assert.Equal("21", seq);
        }

        [Fact]
        public void Adapts_client_provisioner_into_server()
        {
            var p = new Mock<HarshProvisioner>();

            var composite = new HarshServerCompositeProvisioner()
            {
                Web = ServerOM.Web,
                Provisioners = { p.Object }
            };

            composite.Provision();

            Assert.NotNull(p.Object.Context);
            Assert.NotNull(p.Object.Web);

            p.Object.Context.Load(p.Object.Web, w => w.Url);
            p.Object.Context.ExecuteQuery();

            Assert.Equal(ServerOM.Web.Url, p.Object.Web.Url);
        }

        public SharePointServerFixture ServerOM
        {
            get;
            set;
        }

        public void SetFixture(SharePointServerFixture data)
        {
            ServerOM = data;
        }
    }
}
