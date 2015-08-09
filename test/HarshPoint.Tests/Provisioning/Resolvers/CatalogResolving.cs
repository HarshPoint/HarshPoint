using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning.Resolvers
{
    public class CatalogResolving : ResolverTestBase
    {
        public CatalogResolving(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        public async Task MasterPage_catalog_gets_resolved()
        {
            var catalogs = await ResolveAsync(Resolve.Catalog(ListTemplateType.MasterPageCatalog));

            var catalog = Assert.Single(catalogs);

            Assert.Equal(
                (Int32)(ListTemplateType.MasterPageCatalog),
                await catalog.EnsurePropertyAvailable(l => l.BaseTemplate)
            );
        }

        [Fact]
        public async Task MasterPage_RootFolder_gets_resolved()
        {
            var resolver = Resolve
                .Catalog(ListTemplateType.MasterPageCatalog)
                .RootFolder();

            var folder = Assert.Single(await ResolveAsync(resolver));

            Assert.Contains(
                "/_catalogs/masterpage",
                await folder.EnsurePropertyAvailable(f => f.ServerRelativeUrl)
            );
        }
    }
}
