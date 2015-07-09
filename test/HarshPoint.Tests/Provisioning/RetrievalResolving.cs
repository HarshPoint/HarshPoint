using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning
{
    public class RetrievalResolving : SharePointClientTest
    {
        public RetrievalResolving(SharePointClientFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }

        [Fact]
        public async Task Multiple_equal_includes_dont_fail()
        {
            Fixture.ClientContext.Load(Fixture.Web, w => w.Id, w => w.Id);
            await Fixture.ClientContext.ExecuteQueryAsync();
        }

        [Fact]
        public async Task Title_gets_resolved_with_InternalName()
        {
            var ctx = new ClientObjectResolveContext()
            {
                ProvisionerContext = Fixture.Context
            };

            ctx.Include<Field>(
                f => f.InternalName,
                f => f.Description
            );

            var field = await Resolve.FieldById(HarshBuiltInFieldId.Title).ResolveSingleAsync(
                ctx
            );

            Assert.NotNull(field);
            Assert.True(field.IsPropertyAvailable(f => f.Description));
            Assert.True(field.IsPropertyAvailable(f => f.InternalName));
            Assert.Equal("Title", field.InternalName);
        }
    }
}