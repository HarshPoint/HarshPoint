using HarshPoint.Provisioning;
using HarshPoint.Server.Provisioning;
using Moq;
using Moq.Protected;
using System;
using System.Threading.Tasks;
using Xunit;

namespace HarshPoint.Server.Tests
{
    public class HarshProvisionerExtensionsTests : IClassFixture<SharePointServerFixture>
    {
        public HarshProvisionerExtensionsTests(SharePointServerFixture data)
        {
            ServerOM = data;
        }

        public SharePointServerFixture ServerOM
        {
            get;
            set;
        }
        

        [Fact]
        public async Task ToServerProvisioner_calls_Provision()
        {
            var prov = new Mock<HarshProvisioner>();

            prov.Protected()
                .Setup<Task>("InitializeAsync")
                .Returns(HarshTask.Completed)
                .Verifiable();

            prov.Protected()
                .Setup<Task>("OnProvisioningAsync")
                .Returns(HarshTask.Completed)
                .Verifiable();

            prov.Protected()
                .Setup("Complete")
                .Verifiable();

            var serverProv = prov.Object.ToServerProvisioner();

            await serverProv.ProvisionAsync(ServerOM.WebContext);

            prov.Verify();
        }

        [Fact]
        public async Task ToServerProvisioner_calls_Unprovision()
        {
            var clientProv = new Mock<HarshProvisioner>();

            clientProv.Protected()
                .Setup<Task>("InitializeAsync")
                .Returns(HarshTask.Completed)
                .Verifiable();

            clientProv.Protected()
                .Setup<Task>("OnUnprovisioningAsync")
                .Returns(HarshTask.Completed)
                .Verifiable();

            clientProv.Protected()
                .Setup("Complete")
                .Verifiable();

            var serverProv = clientProv.Object.ToServerProvisioner();

            var ctx = (HarshServerProvisionerContext)ServerOM.WebContext.Clone();
            ctx.MayDeleteUserData = true;

            await serverProv.UnprovisionAsync(ctx);

            clientProv.Verify();
        }

        [Fact]
        public void ToServerProvisioner_sets_correct_Web()
        {
            var clientProv = new Mock<HarshProvisioner>();
            clientProv.Protected()
                .Setup<Task>("OnProvisioningAsync")
                .Returns(async () =>
                {
                    clientProv.Object.ClientContext.Load(clientProv.Object.Web, w => w.Url);
                    await clientProv.Object.ClientContext.ExecuteQueryAsync();

                    Assert.Equal(ServerOM.Web.Url, clientProv.Object.Web.Url, StringComparer.OrdinalIgnoreCase);
                });

            var serverProv = clientProv.Object.ToServerProvisioner();
            serverProv.ProvisionAsync(ServerOM.WebContext).Wait();
        }

        [Fact]
        public void ToServerProvisioner_sets_correct_Site()
        {
            var clientProv = new Mock<HarshProvisioner>();
            clientProv.Protected()
                .Setup<Task>("OnProvisioningAsync")
                .Returns(async () =>
                {
                    clientProv.Object.ClientContext.Load(clientProv.Object.Site, s => s.Url);
                    await clientProv.Object.ClientContext.ExecuteQueryAsync();

                    Assert.Equal(ServerOM.Site.Url, clientProv.Object.Site.Url, StringComparer.OrdinalIgnoreCase);
                });

            var serverProv = clientProv.Object.ToServerProvisioner();
            serverProv.ProvisionAsync(ServerOM.WebContext).Wait();
        }

        [Fact]
        public void ToServerProvisioner_fails_with_WebApp()
        {
            var clientProv = Mock.Of<HarshProvisioner>();
            var serverProv = clientProv.ToServerProvisioner();

            var serverContext = new HarshServerProvisionerContext(ServerOM.WebApplication);
            Assert.ThrowsAsync<InvalidOperationException>(() => serverProv.ProvisionAsync(serverContext));
        }

        [Fact]
        public void ToServerProvisioner_fails_with_Farm()
        {
            var clientProv = Mock.Of<HarshProvisioner>();
            var serverProv = clientProv.ToServerProvisioner();

            var serverContext = new HarshServerProvisionerContext(ServerOM.Farm);
            Assert.ThrowsAsync<InvalidOperationException>(() => serverProv.ProvisionAsync(serverContext));
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
