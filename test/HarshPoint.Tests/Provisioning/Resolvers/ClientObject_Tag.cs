using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning.Resolvers
{
    public class ClientObject_Tag : SharePointClientTest
    {
        public ClientObject_Tag(ITestOutputHelper output) : base(output)
        {
        }


        [FactNeedsSharePoint]
        public async Task Contains_NestedResolveResult_for_field()
        {
            var list = await CreateList(l => l.Id);

            var resolver = Resolve
                .List().ById(list.Id)
                .Field().ByInternalName("Title");

            var field = ManualResolver.ResolveSingle(resolver);
            await ClientContext.ExecuteQueryAsync();

            var nrr = field.Value.Tag as NestedResolveResult;
            Assert.NotNull(nrr);

            var tagList = Assert.Single(nrr.Parents.OfType<List>());

            Assert.NotNull(tagList);
            Assert.Equal(list.Id, tagList.Id);
        }

    }
}
