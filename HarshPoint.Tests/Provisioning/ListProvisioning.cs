using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HarshPoint.Tests.Provisioning
{
    public class ListProvisioning : IClassFixture<SharePointClientFixture>
    {
        public ListProvisioning(SharePointClientFixture fix)
        {
            Fixture = fix;
        }

        public SharePointClientFixture Fixture { get; private set; }

        [Fact]
        public async Task Existing_list_is_not_added()
        {
            var prov = new HarshList()
            {
                Title = "Documents",
                Url = "Shared Documents",
                TemplateType = ListTemplateType.DocumentLibrary
            };

            await prov.ProvisionAsync(Fixture.Context);

            Fixture.ClientContext.Load(
                prov.List, 
                l => l.Title, 
                l => l.BaseTemplate
            );

            await Fixture.ClientContext.ExecuteQueryAsync();

            Assert.False(prov.ListAdded);
            Assert.NotNull(prov.List);
            Assert.Equal("Documents", prov.List.Title);
            Assert.Equal((Int32)ListTemplateType.DocumentLibrary, prov.List.BaseTemplate);
        }
    }
}
