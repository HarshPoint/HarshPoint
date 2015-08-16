using HarshPoint.Server.Provisioning;
using Moq;
using Moq.Protected;
using System;
using System.Threading.Tasks;
using Xunit;

namespace HarshPoint.Server.Tests
{
    [Trait("Category", "HarshPoint.Server")]
    public class HarshServerProvisionerTests : IClassFixture<SharePointServerFixture>
    {
        private readonly MockRepository _mockRepo = new MockRepository(MockBehavior.Loose)
        {
            CallBase = true
        };

        public HarshServerProvisionerTests(SharePointServerFixture fixture)
        {
            SPFixture = fixture;

        }
        public SharePointServerFixture SPFixture
        {
            get;
            set;
        }

        [FactNeedsFarm]
        public async Task Provision_calls_Initialize()
        {
            var mock = _mockRepo.Create<HarshServerProvisioner>();

            mock.Protected()
                .Setup<Task>("InitializeAsync")
                .Returns(HarshTask.Completed)
                .Verifiable();

            await mock.Object.ProvisionAsync(SPFixture.WebContext);
            mock.Verify();
        }

        [FactNeedsFarm]
        public async Task Provision_calls_OnProvisioning()
        {
            var mock = _mockRepo.Create<HarshServerProvisioner>();

            mock.Protected()
                .Setup<Task>("OnProvisioningAsync")
                .Returns(HarshTask.Completed)
                .Verifiable();

            await mock.Object.ProvisionAsync(SPFixture.WebContext);
            mock.Verify();
        }

        [FactNeedsFarm]
        public void Provision_always_calls_Complete()
        {
            var mock = _mockRepo.Create<HarshServerProvisioner>();

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

        [FactNeedsFarm]
        public async Task Unprovision_calls_Initialize()
        {
            var mock = _mockRepo.Create<HarshServerProvisioner>();

            mock.Protected()
                .Setup<Task>("InitializeAsync")
                .Returns(HarshTask.Completed)
                .Verifiable();

            await mock.Object.UnprovisionAsync(SPFixture.WebContext);
            mock.Verify();
        }

        [FactNeedsFarm]
        public async Task Unprovision_calls_OnUnprovisioning()
        {
            var mock = _mockRepo.Create<HarshServerProvisioner>();

            mock.Protected()
                .Setup<Task>("OnUnprovisioningAsync")
                .Returns(HarshTask.Completed)
                .Verifiable();

            await mock.Object.UnprovisionAsync(SPFixture.WebContext);
            mock.Verify();
        }

        [FactNeedsFarm]
        public void Unprovision_always_calls_Complete()
        {
            var mock = _mockRepo.Create<HarshServerProvisioner>();

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
