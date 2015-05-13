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

            var ctx = (HarshProvisionerContext)ClientOM.Context.Clone();
            ctx.MayDeleteUserData = true;

            var composite = new HarshProvisioner()
            {
                Children = { p1.Object, p2.Object }
            };
            composite.Provision(ctx);

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

            var ctx = (HarshProvisionerContext)ClientOM.Context.Clone();
            ctx.MayDeleteUserData = true;

            var composite = new HarshProvisioner()
            {
                Children = { p1.Object, p2.Object }
            };
            composite.Unprovision(ctx);

            Assert.Equal("21", seq);
        }

        [Fact]
        public void Assigns_context_to_children()
        {
            var p = new Mock<HarshProvisioner>();
            p.Protected().Setup("OnProvisioning").Callback(() =>
            {
                Assert.Equal(ClientOM.Web, p.Object.Web);
            });

            var composite = new HarshProvisioner()
            {
                Children = { p.Object }
            };
            composite.Provision(ClientOM.Context);
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
