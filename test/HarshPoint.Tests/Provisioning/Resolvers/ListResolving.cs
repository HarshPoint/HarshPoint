using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning.Resolvers
{
    public class ListResolving : ResolverTestBase
    {
        public ListResolving(ITestOutputHelper output) 
            : base(output)
        {
        }

        [Fact]
        public async Task List_gets_resolved_by_url()
        {
            await Fixture.EnsureTestList();

            var results = ManualResolver.Resolve(
                Resolve.List().ByUrl(SharePointClientFixture.TestListUrl)
            );

            await ClientContext.ExecuteQueryAsync();

            var list = Assert.Single(results);

            Assert.NotNull(list);
            Assert.EndsWith(
                "/" + SharePointClientFixture.TestListUrl,
                list.RootFolder.ServerRelativeUrl
            );
        }

        [Fact]
        public async Task Documents_RootFolder_gets_resolved_by_url()
        {
            await Fixture.EnsureTestList();

            var resolver = Resolve.List().ByUrl(SharePointClientFixture.TestListUrl).RootFolder();
            var folder = Assert.Single(await ResolveAsync(resolver));

            Assert.NotNull(folder);
            Assert.EndsWith(
                "/" + SharePointClientFixture.TestListUrl,
                await folder.EnsurePropertyAvailable(f => f.ServerRelativeUrl)
            );
        }
    }
}
