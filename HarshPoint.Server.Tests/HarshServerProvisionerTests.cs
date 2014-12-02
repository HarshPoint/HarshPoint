using HarshPoint.Server.Provisioning;
using Microsoft.SharePoint;
using Moq;
using Moq.Protected;
using System;
using Xunit;

namespace HarshPoint.Server.Tests.UnitTests
{
    public class HarshServerProvisionerTests : IUseFixture<SharePointServerFixture>
    {
        public SharePointServerFixture SPFixture
        {
            get;
            set;
        }

        public void SetFixture(SharePointServerFixture data)
        {
            SPFixture = data;
        }

        [Fact]
        public void Provision_calls_Initialize()
        {
            var mock = new Mock<HarshServerProvisioner>();

            mock.Protected().Setup("Initialize");
            mock.Object.Provision();
            mock.Verify();
        }
    
        [Fact]
        public void Provision_calls_OnProvisioning()
        {
            var mock = new Mock<HarshServerProvisioner>();

            mock.Protected().Setup("OnProvisioning");
            mock.Object.Provision();
            mock.Verify();
        }

        [Fact]
        public void Provision_always_calls_Complete()
        {
            var mock = new Mock<HarshServerProvisioner>();
            
            mock.Protected().Setup("OnProvisioning").Throws<Exception>();
            mock.Protected().Setup("Complete");

            Assert.Throws<Exception>(delegate
            {
                mock.Object.Provision();
            });

            mock.Verify();
        }

        [Fact]
        public void Unprovision_calls_Initialize()
        {
            var mock = new Mock<HarshServerProvisioner>();

            mock.Protected().Setup("Initialize");
            mock.Object.Unprovision();
            mock.Verify();
        }

        [Fact]
        public void Unprovision_calls_OnUnprovisioning()
        {
            var mock = new Mock<HarshServerProvisioner>();

            mock.Protected().Setup("OnUnprovisioning");
            mock.Object.Unprovision();
            mock.Verify();
        }

        [Fact]
        public void Unprovision_always_calls_Complete()
        {
            var mock = new Mock<HarshServerProvisioner>();
            
            mock.Protected().Setup("OnUnprovisioning").Throws<Exception>();
            mock.Protected().Setup("Complete");

            Assert.Throws<Exception>(delegate
            {
                mock.Object.Unprovision();
            });

            mock.Verify();
        }

        [Fact]
        public void Set_Web_sets_Site()
        {
            var p = Mock.Of<HarshServerProvisioner>();
            p.Context = SPFixture.WebContext;
            Assert.Equal(SPFixture.Site, p.Site);
        }

        [Fact]
        public void Set_Web_sets_WebApplication()
        {
            var p = Mock.Of<HarshServerProvisioner>();
            p.Context = SPFixture.WebContext;
            Assert.Equal(SPFixture.WebApplication, p.WebApplication);
        }

        [Fact]
        public void Set_WebContext_sets_Farm()
        {
            var p = Mock.Of<HarshServerProvisioner>();
            p.Context = SPFixture.WebContext;
            Assert.Equal(SPFixture.Farm, p.Farm);
        }

        [Fact]
        public void Set_SiteContext_sets_Web_to_RootWeb()
        {
            var p = Mock.Of<HarshServerProvisioner>();
            p.Context = new HarshServerProvisionerContext(SPFixture.Site);
            Assert.Equal(SPFixture.Site.RootWeb, p.Web);
        }

        [Fact]
        public void Set_SiteContext_sets_WebApplication()
        {
            var p = Mock.Of<HarshServerProvisioner>();
            p.Context = new HarshServerProvisionerContext(SPFixture.Site);
            Assert.Equal(SPFixture.WebApplication, p.WebApplication);
        }

        [Fact]
        public void Set_SiteContext_sets_Farm()
        {
            var p = Mock.Of<HarshServerProvisioner>();
            p.Context = new HarshServerProvisionerContext(SPFixture.Site);
            Assert.Equal(SPFixture.Farm, p.Farm);
        }

        [Fact]
        public void Set_WebApplicationContext_clears_Web()
        {
            var p = Mock.Of<HarshServerProvisioner>();
            p.Context = new HarshServerProvisionerContext(SPFixture.WebApplication);
            Assert.Equal(null, p.Web);
        }

        [Fact]
        public void Set_WebApplicationContext_clears_Site()
        {
            var p = Mock.Of<HarshServerProvisioner>();
            p.Context = new HarshServerProvisionerContext(SPFixture.WebApplication);
            Assert.Equal(null, p.Site);
        }

        [Fact]
        public void Set_WebApplication_sets_Farm()
        {
            var p = Mock.Of<HarshServerProvisioner>();
            p.Context = new HarshServerProvisionerContext(SPFixture.Farm);
            Assert.Equal(SPFixture.Farm, p.Farm);
        }
    }
}
