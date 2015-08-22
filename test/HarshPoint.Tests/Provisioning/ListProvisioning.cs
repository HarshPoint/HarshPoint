using HarshPoint.Provisioning;
using HarshPoint.Provisioning.ProgressReporting;
using Microsoft.SharePoint.Client;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning
{
    public class ListProvisioning : SharePointClientTest
    {
        public ListProvisioning(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        public async Task Existing_list_is_not_added()
        {
            ClientContext.Load(Web, w => w.ServerRelativeUrl);

            var list = await CreateList(
                l => l.Title,
                l => l.RootFolder.ServerRelativeUrl
            );

            var prov = new HarshList()
            {
                Title = list.Title,
                Url = HarshUrl.GetRelativeTo(list.RootFolder.ServerRelativeUrl, Web.ServerRelativeUrl),
            };

            await prov.ProvisionAsync(Context);

            var output = LastObjectOutput<List>();
            Assert.False(output.ObjectAdded);

            var outputList = output.Object;
            Assert.NotNull(outputList);

            ClientContext.Load(
                outputList,
                l => l.Title
            );

            await ClientContext.ExecuteQueryAsync();
            Assert.Equal(list.Title, outputList.Title);
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

            await prov.ProvisionAsync(Context);

            var objectCreated = LastObjectOutput<List>();
            Assert.IsType<ObjectAdded<List>>(objectCreated);

            var list = objectCreated.Object;
            Assert.NotNull(list);

            RegisterForDeletion(list);

            ClientContext.Load(
                list,
                l => l.Title,
                l => l.BaseTemplate
            );

            await ClientContext.ExecuteQueryAsync();

            Assert.NotNull(list);
            Assert.Equal(name, list.Title);
            Assert.Equal((Int32)ListTemplateType.GenericList, list.BaseTemplate);

        }
    }
}
