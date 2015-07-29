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
            Binder = new ResolvedPropertyBinder(GetType());
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
            var ctx = new ClientObjectResolveContext(Fixture.Context);

            ctx.Include<Field>(
                f => f.InternalName,
                f => f.Description
            );

            Field = Resolve.Field().ById(HarshBuiltInFieldId.Title);

            Binder.Bind(this, () => ctx);

            await ctx.ProvisionerContext.ClientContext.ExecuteQueryAsync();

            var field = Field.Value;

            Assert.NotNull(field);
            Assert.True(field.IsPropertyAvailable(f => f.Description));
            Assert.True(field.IsPropertyAvailable(f => f.InternalName));
            Assert.Equal("Title", field.InternalName);
        }

        public IResolveSingle<Field> Field { get; set; }
        private ResolvedPropertyBinder Binder { get; set; }
    }
}