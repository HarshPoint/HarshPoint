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

            serverProv.Provision(ServerOM.WebContext);

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
            serverProv.Unprovision(ServerOM.WebContext);

            clientProv.Verify();
        }

        [Fact]
        public void ToServerProvisioner_sets_correct_Web()
        {
            var clientProv = new Mock<HarshProvisioner>();
            clientProv.Protected().Setup("OnProvisioning").Callback(() =>
            {
                clientProv.Object.ClientContext.Load(clientProv.Object.Web, w => w.Url);
                clientProv.Object.ClientContext.ExecuteQuery();

                Assert.Equal(ServerOM.Web.Url, clientProv.Object.Web.Url, StringComparer.OrdinalIgnoreCase);

            });
            var serverProv = clientProv.Object.ToServerProvisioner();
            serverProv.Provision(ServerOM.WebContext);
        }

        [Fact]
        public void ToServerProvisioner_sets_correct_Site()
        {
            var clientProv = new Mock<HarshProvisioner>();
            clientProv.Protected().Setup("OnProvisioning").Callback(() =>
            {
                clientProv.Object.ClientContext.Load(clientProv.Object.Site, s => s.Url);
                clientProv.Object.ClientContext.ExecuteQuery();

                Assert.Equal(ServerOM.Site.Url, clientProv.Object.Site.Url, StringComparer.OrdinalIgnoreCase);
            });

            var serverProv = clientProv.Object.ToServerProvisioner();
            serverProv.Provision(ServerOM.WebContext);
        }

        [Fact]
        public void ToServerProvisioner_fails_with_WebApp()
        {
            var clientProv = Mock.Of<HarshProvisioner>();
            var serverProv = clientProv.ToServerProvisioner();

            var serverContext = new HarshServerProvisionerContext(ServerOM.WebApplication);
            Assert.Throws<InvalidOperationException>(() => serverProv.Provision(serverContext));
        }

        [Fact]
        public void ToServerProvisioner_fails_with_Farm()
        {
            var clientProv = Mock.Of<HarshProvisioner>();
            var serverProv = clientProv.ToServerProvisioner();

            var serverContext = new HarshServerProvisionerContext(ServerOM.Farm);
            Assert.Throws<InvalidOperationException>(() => serverProv.Provision(serverContext));
        }

        [Fact]
        public void ToServerProvisioner_cannot_have_children()
        {
            var clientProv = new HarshProvisioner();
            var serverProv = clientProv.ToServerProvisioner();

            Assert.Throws<NotSupportedException>(() =>
                serverProv.Children.Add(new HarshServerProvisioner())
            );
        }
    }
}
