using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HarshPoint.Tests.Provisioning.Resolvers
{
    public class CatalogResolving : IClassFixture<SharePointClientFixture>
    {
        public CatalogResolving(SharePointClientFixture fixture)
        {
            ClientOM = fixture;
        }

        public SharePointClientFixture ClientOM { get; private set; }

        [Fact]
        public async Task MasterPage_catalog_gets_resolved()
        {
            var resolver = Resolve.Catalog(ListTemplateType.MasterPageCatalog);
            var catalogs = await ClientOM.Context.ResolveAsync(resolver);

            var catalog = Assert.Single(catalogs);

            Assert.Equal(
                (Int32)(ListTemplateType.MasterPageCatalog), 
                await catalog.EnsurePropertyAvailable(l => l.BaseTemplate)
            );
        }

        public async Task MasterPage_RootFolder_gets_resolved()
        {
            IResolveSingle<Folder> resolver = Resolve
                .Catalog(ListTemplateType.MasterPageCatalog)
                .RootFolder();

            var folder = await ClientOM.Context.ResolveSingleAsync(resolver);

            Assert.Contains(
                "/_catalogs/masterpage",
                await folder.EnsurePropertyAvailable(f => f.ServerRelativeUrl)
            );
        }
    }
}
