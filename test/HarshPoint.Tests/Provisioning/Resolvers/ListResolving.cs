using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System;
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
            ClientContext.Load(Web, w => w.ServerRelativeUrl);
            var list = await CreateList(l => l.RootFolder.ServerRelativeUrl);

            var url = HarshUrl.GetRelativeTo(list.RootFolder.ServerRelativeUrl, Web.ServerRelativeUrl);

            var results = ManualResolver.Resolve(
                Resolve.List().ByUrl(url)
            );

            await ClientContext.ExecuteQueryAsync();

            var resolvedList = Assert.Single(results);

            Assert.NotNull(resolvedList);
            Assert.Equal(
                list.RootFolder.ServerRelativeUrl,
                resolvedList.RootFolder.ServerRelativeUrl,
                StringComparer.OrdinalIgnoreCase
            );
        }

        [Fact]
        public async Task Documents_RootFolder_gets_resolved_by_url()
        {
            var list = await CreateList(l => l.RootFolder.ServerRelativeUrl);

            var resolver = Resolve.List().ById(list.Id).RootFolder();
            var folder = Assert.Single(await ResolveAsync(resolver));

            Assert.NotNull(folder);
            Assert.Equal(
                list.RootFolder.ServerRelativeUrl,
                await folder.EnsurePropertyAvailable(f => f.ServerRelativeUrl)
            );
        }
    }
}
