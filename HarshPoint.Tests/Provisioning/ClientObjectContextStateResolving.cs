using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System.Threading.Tasks;
using Xunit;

namespace HarshPoint.Tests.Provisioning
{
    public class ClientObjectContextStateResolving : IClassFixture<SharePointClientFixture>
    {
        public ClientObjectContextStateResolving(SharePointClientFixture fixture)
        {
            Fixture = fixture;
        }

        public SharePointClientFixture Fixture { get; private set; }

        [Fact]
        public async Task Resolves_object_ensures_retrievals()
        {
            var web = Fixture.ClientContext.Web;
            Assert.False(web.IsPropertyAvailable(w => w.SiteLogoUrl));

            var ctx = Fixture.Context.PushState(Fixture.ClientContext.Web);

            var resolveCtx = new ClientObjectResolveContext<Web>(w => w.SiteLogoUrl)
            {
                ProvisionerContext = ctx,
            };

            var resolver = new ClientObjectContextStateResolver<Web>();
            var resolvedWeb = await resolver.ResolveSingleAsync(resolveCtx);

            Assert.True(resolvedWeb.IsPropertyAvailable(w => w.SiteLogoUrl));
        }

        [Fact]
        public async Task Resolves_object_ensures_collection_retrieval()
        {
            var web = Fixture.ClientContext.Web;
            Assert.False(web.Lists.ServerObjectIsNull.HasValue);

            var ctx = Fixture.Context.PushState(Fixture.ClientContext.Web);

            var resolveCtx = new ClientObjectResolveContext<Web>(w => w.Lists.Include(l => l.ItemCount))
            {
                ProvisionerContext = ctx,
            };

            var resolver = new ClientObjectContextStateResolver<Web>();
            var resolvedWeb = await resolver.ResolveSingleAsync(resolveCtx);

            Assert.True(resolvedWeb.Lists.ServerObjectIsNull.HasValue);
            Assert.False(resolvedWeb.Lists.ServerObjectIsNull.Value);
            Assert.NotEmpty(resolvedWeb.Lists);

            Assert.All(resolvedWeb.Lists, 
                list => Assert.True(
                    list.IsPropertyAvailable(l => l.ItemCount)
                )
            );
        }
    }
}
