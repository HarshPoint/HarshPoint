using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning.Resolvers
{
    public class ListResolving : SharePointClientTest
    {
        public ListResolving(SharePointClientFixture fixture, ITestOutputHelper output) 
            : base(fixture, output)
        {
        }

        [Fact]
        public async Task Documents_list_gets_resolved_by_url()
        {
            await Fixture.EnsureTestList();

            var resolver = (IResolve<List>)Resolve.ListByUrl(SharePointClientFixture.TestListUrl);
            var list = Assert.Single(await resolver.TryResolveAsync(Fixture.ResolveContext));

            Assert.NotNull(list);
            Assert.EndsWith(
                "/" + SharePointClientFixture.TestListUrl,
                await list.RootFolder.EnsurePropertyAvailable(f => f.ServerRelativeUrl)
            );
        }

        [Fact]
        public async Task Documents_RootFolder_gets_resolved_by_url()
        {
            await Fixture.EnsureTestList();

            var resolver = (IResolve<Folder>)Resolve.ListByUrl(SharePointClientFixture.TestListUrl).RootFolder();
            var folder = Assert.Single(await resolver.TryResolveAsync(Fixture.ResolveContext));

            Assert.NotNull(folder);
            Assert.EndsWith(
                "/" + SharePointClientFixture.TestListUrl,
                await folder.EnsurePropertyAvailable(f => f.ServerRelativeUrl)
            );
        }
    }
}
