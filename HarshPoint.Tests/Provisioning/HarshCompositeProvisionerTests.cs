using HarshPoint.Provisioning;
using Moq;
using Moq.Protected;
using System;
using Xunit;

namespace HarshPoint.Tests.Provisioning
{
    public class HarshCompositeProvisionerTests : IUseFixture<SharePointClientFixture>
    {
        [Fact]
        public void Calls_children_provision_in_correct_order()
        {
            var seq = String.Empty;

            var p1 = new Mock<HarshProvisioner>();
            var p2 = new Mock<HarshProvisioner>();

            p1.Protected().Setup("OnProvisioning").Callback(() => seq += "1");
            p2.Protected().Setup("OnProvisioning").Callback(() => seq += "2");

            var composite = new HarshCompositeProvisioner()
             {
                 Provisioners = { p1.Object, p2.Object }
             };

            composite.Provision();

            Assert.Equal("12", seq);
        }

        [Fact]
        public void Calls_children_unprovision_in_correct_order()
        {
            var seq = String.Empty;

            var p1 = new Mock<HarshProvisioner>();
            var p2 = new Mock<HarshProvisioner>();

            p1.Protected().Setup("OnUnprovisioning").Callback(() => seq += "1");
            p2.Protected().Setup("OnUnprovisioning").Callback(() => seq += "2");

            var composite = new HarshCompositeProvisioner()
             {
                 Provisioners = { p1.Object, p2.Object }
             };

            composite.Unprovision();

            Assert.Equal("21", seq);
        }

        [Fact]
        public void Assigns_context_to_children()
        {
            var p = new Mock<HarshProvisioner>();

            var composite = new HarshCompositeProvisioner()
            {
                Context = ClientOM.Context,
                Provisioners = { p.Object }
            };

            composite.Provision();

            Assert.Equal(ClientOM.Web, p.Object.Web);
        }

        public SharePointClientFixture ClientOM
        {
            get;
            set;
        }

        public void SetFixture(SharePointClientFixture data)
        {
            ClientOM = data;
        }
    }
}
