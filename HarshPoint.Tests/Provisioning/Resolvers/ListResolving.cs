using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System.Threading.Tasks;
using Xunit;

namespace HarshPoint.Tests.Provisioning.Resolvers
{
    public class ListResolving : IClassFixture<SharePointClientFixture>
    {
        public ListResolving(SharePointClientFixture fixture)
        {
            ClientOM = fixture;
        }

        public SharePointClientFixture ClientOM { get; private set; }

        [Fact]
        public async Task Documents_list_gets_resolved_by_url()
        {
            var resolver = (IResolveSingle<List>)Resolve.ListByUrl("Documents");
            var list = await resolver.ResolveSingleAsync(ClientOM.Context);

            Assert.NotNull(list);
            Assert.EndsWith(
                "/Documents",
                await list.RootFolder.EnsurePropertyAvailable(f => f.ServerRelativeUrl)
            );
        }

        [Fact]
        public async Task Documents_RootFolder_gets_resolved_by_url()
        {
            var resolver = (IResolveSingle<Folder>)Resolve.ListByUrl("Documents").RootFolder();
            var folder = await resolver.ResolveSingleAsync(ClientOM.Context);

            Assert.NotNull(folder);
            Assert.EndsWith(
                "/Documents",
                await folder.EnsurePropertyAvailable(f => f.ServerRelativeUrl)
            );
        }
    }
}
