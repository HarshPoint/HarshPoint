using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning.Resolvers
{
    public class FieldResolving : SharePointClientTest
    {
        public FieldResolving(ITestOutputHelper output) 
            : base(output)
        {
        }

        [Fact]
        public async Task Documents_Title_field_gets_resolved_by_id()
        {
            await Fixture.EnsureTestList();

            var resolver = ManualResolver.ResolveSingleOrDefault(
                Resolve
                .List().ByUrl(SharePointClientFixture.TestListUrl)
                .Field().ById(HarshBuiltInFieldId.Title)
            );

            await ClientContext.ExecuteQueryAsync();

            var field = resolver.Value;

            Assert.NotNull(field);
            Assert.Equal("Title", await field.EnsurePropertyAvailable(f => f.InternalName));
        }
    }
}
