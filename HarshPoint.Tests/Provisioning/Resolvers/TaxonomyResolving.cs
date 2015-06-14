using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace HarshPoint.Tests.Provisioning.Resolvers
{
    public class TaxonomyResolving : IClassFixture<SharePointClientFixture>
    {
        private static readonly Guid GroupId = new Guid("6e3c51c5-9199-4b0e-bb81-00b8ec433740");
        private static readonly Guid TermSetId = new Guid("a5d59d30-10e8-4221-bf59-75a1d76f0be0");

        public TaxonomyResolving(SharePointClientFixture fix)
        {
            Fixture = fix;
        }

        public SharePointClientFixture Fixture { get; private set; }

        [Fact]
        public async Task Default_SiteCollection_TermStore_gets_resolved()
        {
            var resolver = Resolve.TermStoreSiteCollectionDefault();
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
            var resolver = Resolve.TermStoreKeywordsDefault();
            var termStore = await resolver.ResolveSingleAsync(Fixture.ResolveContext);
            Assert.NotNull(termStore);

            var expected = Fixture.TaxonomySession.GetDefaultKeywordsTermStore();

            Fixture.ClientContext.Load(expected, ts => ts.Id);
            Fixture.ClientContext.Load(termStore, ts => ts.Id);
            await Fixture.ClientContext.ExecuteQueryAsync();

            Assert.Equal(expected.Id, termStore.Id);
        }

        [Fact]
        public async Task TermSet_resolved_by_id()
        {
            var expected = await EnsureTestTermSet();

            IResolve<TermSet> resolver = Resolve.TermStoreSiteCollectionDefault().TermSetById(TermSetId);
            var actual = await resolver.ResolveSingleAsync(Fixture.ResolveContext);

            Fixture.ClientContext.Load(actual, ts => ts.Id, ts => ts.Name);
            await Fixture.ClientContext.ExecuteQueryAsync();

            Assert.Equal(TermSetId, actual.Id);
            Assert.Equal(TermSetId.ToString("n"), actual.Name);
        }

        private async Task<TermSet> EnsureTestTermSet()
        {
            var store = Fixture.TaxonomySession.GetDefaultSiteCollectionTermStore();
            var group = store.GetGroup(GroupId);

            try
            {
                await Fixture.ClientContext.ExecuteQueryAsync();
            }
            catch (ArgumentOutOfRangeException)
            {
                if (group.IsNull())
                {
                    group = store.CreateGroup(GroupId.ToString("n"), GroupId);
                }
            }

            var termSet = group.TermSets.GetById(TermSetId);

            try
            {
                await Fixture.ClientContext.ExecuteQueryAsync();
            }
            catch (ArgumentOutOfRangeException)
            {
                if (termSet.IsNull())
                {
                    termSet = group.CreateTermSet(TermSetId.ToString("n"), TermSetId, 1033);
                }

                await Fixture.ClientContext.ExecuteQueryAsync();
            }

            return termSet;
        }
    }
}
