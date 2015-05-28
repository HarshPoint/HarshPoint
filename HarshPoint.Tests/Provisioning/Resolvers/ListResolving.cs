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
            var list = await ClientOM.Context.ResolveSingleAsync(
                Resolve.ListByUrl("Documents")
            );

            Assert.NotNull(list);
            Assert.EndsWith(
                "/Documents",
                await list.RootFolder.EnsurePropertyAvailable(f => f.ServerRelativeUrl)
            );
        }

        [Fact]
        public async Task Documents_RootFolder_gets_resolved_by_url()
        {
            var folder = await ClientOM.Context.ResolveSingleAsync<Folder>(
                Resolve.ListByUrl("Documents").RootFolder()
            );

            Assert.NotNull(folder);
            Assert.EndsWith(
                "/Documents",
                await folder.EnsurePropertyAvailable(f => f.ServerRelativeUrl)
            );
        }
    }
}
