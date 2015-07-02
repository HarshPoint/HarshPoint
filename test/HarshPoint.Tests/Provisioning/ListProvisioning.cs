using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning
{
    public class ListProvisioning : SharePointClientTest
    {
        public ListProvisioning(SharePointClientFixture fixture, ITestOutputHelper output) 
            : base(fixture, output)
        {
        }

        [Fact]
        public async Task Existing_list_is_not_added()
        {
            await Fixture.EnsureTestList();

            var prov = new HarshList()
            {
                Title = SharePointClientFixture.TestListTitle,
                Url = SharePointClientFixture.TestListUrl,
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
            Assert.Equal(SharePointClientFixture.TestListTitle, prov.List.Title);
            Assert.Equal((Int32)ListTemplateType.DocumentLibrary, prov.List.BaseTemplate);
        }

        [Fact]
        public async Task Random_list_is_added()
        {
            var name = Guid.NewGuid().ToString("n");

            var prov = new HarshList()
            {
                Title = name,
                Url = "Lists/" + name,
            };

            try
            {
                await prov.ProvisionAsync(Fixture.Context);

                Fixture.ClientContext.Load(
                    prov.List,
                    l => l.Title,
                    l => l.BaseTemplate
                );

                await Fixture.ClientContext.ExecuteQueryAsync();

                Assert.True(prov.ListAdded);
                Assert.NotNull(prov.List);
                Assert.Equal(name, prov.List.Title);
                Assert.Equal((Int32)ListTemplateType.GenericList, prov.List.BaseTemplate);
            }
            finally
            {
                prov.List.DeleteObject();
                await Fixture.ClientContext.ExecuteQueryAsync();
            }
        }
    }
}
