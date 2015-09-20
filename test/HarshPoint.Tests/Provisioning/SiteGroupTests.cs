using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning
{
    public class SiteGroupTests : SharePointClientTest
    {
        private String _guid;

        public SiteGroupTests(ITestOutputHelper output) : base(output)
        {
            _guid = Guid.NewGuid().ToString("n");
        }

        [FactNeedsSharePoint]
        public async Task Existing_SiteGroup_is_not_provisioned()
        {
            var group = await CreateSiteGroup();

            var prov = new HarshSiteGroup()
            {
                Name = group.Title,
            };

            await prov.ProvisionAsync(Context);

            var output = LastObjectOutput<Group>();
            Assert.False(output.ObjectAdded);
        }

        [FactNeedsSharePoint]
        public async Task SiteGroup_gets_provisioned()
        {
            var prov = new HarshSiteGroup()
            {
                Name = _guid,
                Description = _guid,
            };

            Group g = null;

            try
            {
                await prov.ProvisionAsync(Context);

                var output = LastObjectOutput<Group>();
                g = output.Object;
                Assert.True(output.ObjectAdded);
                Assert.NotNull(g);

                ClientContext.Load(
                    g,
                    c => c.Title,
                    c => c.Description
                );

                await ClientContext.ExecuteQueryAsync();

                Assert.Equal(_guid, g.Title);
                Assert.Equal(_guid, g.Description);
            }
            finally
            {
                if (g != null)
                {
                    ClientContext.Web.SiteGroups.Remove(g);
                    await ClientContext.ExecuteQueryAsync();
                }
            }
        }

        [FactNeedsSharePoint]
        public async Task SiteGroup_gets_unprovisioned()
        {
            var group = await CreateSiteGroup();
            var id = group.Id;

            var prov = new HarshSiteGroup()
            {
                Name = group.Title,
            };

            await prov.UnprovisionAsync(Context.AllowDeleteUserData());

            group = Web.SiteGroups.GetById(id);
            ClientContext.Load(group, g => g.Title);

            await Assert.ThrowsAsync<ServerException>(() => ClientContext.ExecuteQueryAsync());
        }

        [FactNeedsSharePoint]
        public async Task SiteGroup_description_gets_changed()
        {
            var group = await CreateSiteGroup();

            var prov = new HarshSiteGroup()
            {
                Name = group.Title,
                Description = _guid,
            };

            await prov.ProvisionAsync(Context);

            ClientContext.Load(
                group,
                c => c.Description
            );

            await ClientContext.ExecuteQueryAsync();

            Assert.Equal(_guid, group.Description);
        }
    }
}
