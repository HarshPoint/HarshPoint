using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning.Resolvers
{
    public class ListResolving : ResolverTestBase
    {
        public ListResolving(SharePointClientFixture fixture, ITestOutputHelper output) 
            : base(fixture, output)
        {
        }

        [Fact]
        public async Task List_gets_resolved_by_url()
        {
            await Fixture.EnsureTestList();

            var results = await ResolveAsync<List>(
                Resolve.List.ByUrl(SharePointClientFixture.TestListUrl)
            );

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

            var resolver = (IResolveOld<Folder>)Resolve.ListByUrlOld(SharePointClientFixture.TestListUrl).RootFolder();
            var folder = Assert.Single(await resolver.TryResolveAsync(Fixture.ResolveContext));

            Assert.NotNull(folder);
            Assert.EndsWith(
                "/" + SharePointClientFixture.TestListUrl,
                await folder.EnsurePropertyAvailable(f => f.ServerRelativeUrl)
            );
        }
    }
}
