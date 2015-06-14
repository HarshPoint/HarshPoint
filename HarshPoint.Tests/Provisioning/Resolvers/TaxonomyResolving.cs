using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client.Taxonomy;
using System.Threading.Tasks;
using Xunit;

namespace HarshPoint.Tests.Provisioning.Resolvers
{
    public class TaxonomyResolving : IClassFixture<SharePointClientFixture>
    {
        public TaxonomyResolving(SharePointClientFixture fix)
        {
            Fixture = fix;
        }

        public SharePointClientFixture Fixture { get; private set; }

        [Fact]
        public async Task Default_SiteCollection_TermStore_gets_resolved()
        {
            IResolve<TermStore> resolver = Resolve.TermStoreSiteCollectionDefault();
            var termStore = await resolver.ResolveSingleAsync(Fixture.ResolveContext);
            Assert.NotNull(termStore);

            var expected = Fixture.TaxonomySession.GetDefaultSiteCollectionTermStore();

            Fixture.ClientContext.Load(expected, ts => ts.Id);
            Fixture.ClientContext.Load(termStore, ts => ts.Id);
            await Fixture.ClientContext.ExecuteQueryAsync();

            Assert.Equal(expected.Id, termStore.Id);
        }

        [Fact]
        public async Task Default_Keywords_TermStore_gets_resolved()
        {
            IResolve<TermStore> resolver = Resolve.TermStoreKeywordsDefault();
            var termStore = await resolver.ResolveSingleAsync(Fixture.ResolveContext);
            Assert.NotNull(termStore);

            var expected = Fixture.TaxonomySession.GetDefaultKeywordsTermStore();

            Fixture.ClientContext.Load(expected, ts => ts.Id);
            Fixture.ClientContext.Load(termStore, ts => ts.Id);
            await Fixture.ClientContext.ExecuteQueryAsync();

            Assert.Equal(expected.Id, termStore.Id);
        }
    }
}
