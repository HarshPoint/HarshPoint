using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning
{
    public class ClientObjectContextStateResolving : SharePointClientTest
    {
        public ClientObjectContextStateResolving(ITestOutputHelper output)
            : base(output)
        {
        }

        [FactNeedsSharePoint]
        public async Task Resolves_object_ensures_retrievals()
        {
            Assert.False(Web.IsPropertyAvailable(w => w.SiteLogoUrl));

            var ctx = Context.PushState(ClientContext.Web);

            var resolveCtx = new ClientObjectResolveContext(ctx);
            resolveCtx.Include<Web>(w => w.SiteLogoUrl);

            var mr = new ClientObjectManualResolver(() => resolveCtx);
            var resolver = mr.ResolveSingle(
                new ClientObjectContextStateResolveBuilder<Web>()
            );

            await ClientContext.ExecuteQueryAsync();
            var resolvedWeb = resolver.Value;

            Assert.True(resolvedWeb.IsPropertyAvailable(w => w.SiteLogoUrl));
        }

        [FactNeedsSharePoint]
        public async Task Resolves_object_ensures_collection_retrieval()
        {
            Assert.False(Web.Lists.ServerObjectIsNull.HasValue);

            var ctx = Context.PushState(base.Web);

            var resolveCtx = new ClientObjectResolveContext(ctx);
            resolveCtx.Include<Web>(w => w.Lists.Include(l => l.ItemCount));

            var mr = new ClientObjectManualResolver(() => resolveCtx);
            var resolver = mr.ResolveSingle(
                new ClientObjectContextStateResolveBuilder<Web>()
            );

            await ClientContext.ExecuteQueryAsync();

            var resolvedWeb = resolver.Value;

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
