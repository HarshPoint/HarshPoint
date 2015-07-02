using HarshPoint.Provisioning;
using HarshPoint.Server.Provisioning;
using Moq;
using Moq.Protected;
using System;
using System.Threading.Tasks;
using Xunit;

namespace HarshPoint.Server.Tests
{
    public class HarshServerProvisionerTests : IClassFixture<SharePointServerFixture>
    {
        public HarshServerProvisionerTests(SharePointServerFixture fixture)
        {
            SPFixture = fixture;

        }
        public SharePointServerFixture SPFixture
        {
            get;
            set;
        }

        [Fact]
        public async Task Provision_calls_Initialize()
        {
            var mock = new Mock<HarshServerProvisioner>();

            mock.Protected()
                .Setup<Task>("InitializeAsync")
                .Returns(HarshTask.Completed)
                .Verifiable();

            await mock.Object.ProvisionAsync(SPFixture.WebContext);
            mock.Verify();
        }

        [Fact]
        public async Task Provision_calls_OnProvisioning()
        {
            var mock = new Mock<HarshServerProvisioner>();

            mock.Protected()
                .Setup<Task>("OnProvisioningAsync")
                .Returns(HarshTask.Completed)
                .Verifiable();

            await mock.Object.ProvisionAsync(SPFixture.WebContext);
            mock.Verify();
        }

        [Fact]
        public void Provision_always_calls_Complete()
        {
            var mock = new Mock<HarshServerProvisioner>();

            mock.Protected()
                .Setup<Task>("OnProvisioningAsync")
                .Throws<Exception>();

            mock.Protected()
                .Setup("Complete")
                .Verifiable();

            Assert.ThrowsAsync<Exception>(
                () => mock.Object.ProvisionAsync(SPFixture.WebContext)
            );

            mock.Verify();
        }

        [Fact]
        public async Task Unprovision_calls_Initialize()
        {
            var mock = new Mock<HarshServerProvisioner>();

            mock.Protected()
                .Setup<Task>("InitializeAsync")
                .Returns(HarshTask.Completed)
                .Verifiable();

            await mock.Object.UnprovisionAsync(SPFixture.WebContext);
            mock.Verify();
        }

        [Fact]
        public async Task Unprovision_calls_OnUnprovisioning()
        {
            var mock = new Mock<HarshServerProvisioner>();

            mock.Protected()
                .Setup<Task>("OnUnprovisioningAsync")
                .Returns(HarshTask.Completed)
                .Verifiable();

            await mock.Object.UnprovisionAsync(SPFixture.WebContext);
            mock.Verify();
        }

        [Fact]
        public void Unprovision_always_calls_Complete()
        {
            var mock = new Mock<HarshServerProvisioner>();

            mock.Protected().Setup("OnUnprovisioningAsync").Throws<Exception>();
            mock.Protected().Setup("Complete").Verifiable();

            var ctx = SPFixture.WebContext.AllowDeleteUserData();

            Assert.ThrowsAsync<Exception>(
                () => mock.Object.UnprovisionAsync(ctx)
            );

            mock.Verify();
        }

    }
}
