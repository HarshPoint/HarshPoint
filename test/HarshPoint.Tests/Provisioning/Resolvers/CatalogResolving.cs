using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning.Resolvers
{
    public class CatalogResolving : SharePointClientTest
    {
        public CatalogResolving(SharePointClientFixture fixture, ITestOutputHelper output) 
            : base(fixture, output)
        {
        }

        [Fact]
        public async Task MasterPage_catalog_gets_resolved()
        {
            var resolver = (IResolve<List>)Resolve.Catalog(ListTemplateType.MasterPageCatalog);
            var catalogs = await resolver.TryResolveAsync(Fixture.ResolveContext);

            var catalog = Assert.Single(catalogs);

            Assert.Equal(
                (Int32)(ListTemplateType.MasterPageCatalog),
                await catalog.EnsurePropertyAvailable(l => l.BaseTemplate)
            );
        }

        public async Task MasterPage_RootFolder_gets_resolved()
        {
            IResolve<Folder> resolver = Resolve
                .Catalog(ListTemplateType.MasterPageCatalog)
                .RootFolder();

            var folder = Assert.Single(await resolver.TryResolveAsync(Fixture.ResolveContext));

            Assert.Contains(
                "/_catalogs/masterpage",
                await folder.EnsurePropertyAvailable(f => f.ServerRelativeUrl)
            );
        }
    }
}
