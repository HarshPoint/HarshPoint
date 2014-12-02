using HarshPoint.Provisioning;
using HarshPoint.Server.Provisioning;
using Moq;
using Moq.Protected;
using System;
using Xunit;

namespace HarshPoint.Server.Tests
{
    public class HarshProvisionerExtensionsTests : IUseFixture<SharePointServerFixture>
    {
        public SharePointServerFixture ServerOM
        {
            get;
            set;
        }

        public void SetFixture(SharePointServerFixture data)
        {
            ServerOM = data;
        }

        [Fact]
        public void ToServerProvisioner_calls_Provision()
        {
            var prov = new Mock<HarshProvisioner>();
            prov.Protected().Setup("Initialize");
            prov.Protected().Setup("OnProvisioning");
            prov.Protected().Setup("Complete");

            var serverProv = prov.Object.ToServerProvisioner();

            serverProv.Context = ServerOM.WebContext;
            serverProv.Provision();

            prov.Verify();
        }

        [Fact]
        public void ToServerProvisioner_calls_Unprovision()
        {
            var clientProv = new Mock<HarshProvisioner>();
            clientProv.Protected().Setup("Initialize");
            clientProv.Protected().Setup("OnUnprovisioning");
            clientProv.Protected().Setup("Complete");

            var serverProv = clientProv.Object.ToServerProvisioner();
            serverProv.Context = ServerOM.WebContext;
            serverProv.Unprovision();

            clientProv.Verify();
        }

        [Fact]
        public void ToServerProvisioner_sets_correct_Web()
        {
            var clientProv = Mock.Of<HarshProvisioner>();
            var serverProv = clientProv.ToServerProvisioner();

            serverProv.Context = ServerOM.WebContext;
            serverProv.Provision();

            clientProv.Context.Load(clientProv.Web, w => w.Url);
            clientProv.Context.ExecuteQuery();

            Assert.Equal(ServerOM.Web.Url, clientProv.Web.Url, StringComparer.OrdinalIgnoreCase);
        }

        [Fact]
        public void ToServerProvisioner_sets_correct_Site()
        {
            var clientProv = Mock.Of<HarshProvisioner>();
            var serverProv = clientProv.ToServerProvisioner();

            serverProv.Context = ServerOM.WebContext;
            serverProv.Provision();

            clientProv.Context.Load(clientProv.Site, s => s.Url);
            clientProv.Context.ExecuteQuery();

            Assert.Equal(ServerOM.Site.Url, clientProv.Site.Url, StringComparer.OrdinalIgnoreCase);
        }

        [Fact]
        public void ToServerProvisioner_fails_with_WebApp()
        {
            var clientProv = Mock.Of<HarshProvisioner>();
            var serverProv = clientProv.ToServerProvisioner();

            serverProv.Context = new HarshServerProvisionerContext(ServerOM.WebApplication);
            Assert.Throws<InvalidOperationException>(() => serverProv.Provision());
        }

        [Fact]
        public void ToServerProvisioner_fails_with_Farm()
        {
            var clientProv = Mock.Of<HarshProvisioner>();
            var serverProv = clientProv.ToServerProvisioner();

            serverProv.Context = new HarshServerProvisionerContext(ServerOM.Farm);
            Assert.Throws<InvalidOperationException>(() => serverProv.Provision());
        }
    }
}
