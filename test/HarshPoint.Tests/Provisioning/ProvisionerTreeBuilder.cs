using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning
{
    public class ProvisionerTreeBuilder : SharePointClientTest
    {
        public ProvisionerTreeBuilder(SharePointClientFixture fixture, ITestOutputHelper output) : base(fixture, output)
        {
        }

        [Fact]
        public void Adds_sub_provisioner()
        {
            var parent = new HarshProvisioner();
            var child = new HarshProvisioner();

            HarshProvisionerTreeBuilder.AddChild(parent, child);

            Assert.Contains(child, parent.Children);
        }

        [Fact]
        public async Task Adds_default_context_tag()
        {
            var parent = new HarshProvisioner()
            {
                Children = { new ExpectsTag() }
            };

            var tag = new Tag() { Value = 42 };

            HarshProvisionerTreeBuilder.AddChild(parent, tag);

            await parent.ProvisionAsync(Fixture.Context);
        }

        private sealed class ExpectsTag : HarshProvisioner
        {
            [Parameter]
            [DefaultFromContext(typeof(Tag))]
            public Int32? Param
            {
                get;
                set;
            }

            protected override Task OnProvisioningAsync()
            {
                Assert.Equal(42, Param.Value);
                return base.OnProvisioningAsync();
            }
        }

        private sealed class Tag : DefaultFromContextTag<Int32>
        {
        }
    }
}
