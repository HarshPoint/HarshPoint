using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System;
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
            await EnsureDocuments();

            var resolver = (IResolveSingle<List>)Resolve.ListByUrl("Shared Documents");
            var list = await resolver.ResolveSingleAsync(ClientOM.ResolveContext);

            Assert.NotNull(list);
            Assert.EndsWith(
                "/Shared Documents",
                await list.RootFolder.EnsurePropertyAvailable(f => f.ServerRelativeUrl)
            );
        }

        [Fact]
        public async Task Documents_RootFolder_gets_resolved_by_url()
        {
            await EnsureDocuments();

            var resolver = (IResolveSingle<Folder>)Resolve.ListByUrl("Shared Documents").RootFolder();
            var folder = await resolver.ResolveSingleAsync(ClientOM.ResolveContext);

            Assert.NotNull(folder);
            Assert.EndsWith(
                "/Shared Documents",
                await folder.EnsurePropertyAvailable(f => f.ServerRelativeUrl)
            );
        }

        private async Task EnsureDocuments()
        {
            var list = ClientOM.Web.Lists.GetByTitle("Documents");
            ClientOM.ClientContext.Load(list);

            await ClientOM.ClientContext.ExecuteQueryAsync();

            if (list.IsNull())
            {
                list = ClientOM.Web.Lists.Add(new ListCreationInformation()
                {
                    Url = "Shared%20Documents",
                    Title = "Documents",
                    TemplateType = (Int32)ListTemplateType.DocumentLibrary,
                    DocumentTemplateType = (Int32)DocumentTemplateType.Word,
                });

                await ClientOM.ClientContext.ExecuteQueryAsync();
            }

        }
    }
}
