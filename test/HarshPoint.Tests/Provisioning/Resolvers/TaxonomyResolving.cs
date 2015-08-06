using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning.Resolvers
{
    public class TaxonomyResolving : SharePointClientTest
    {
        //private static readonly Guid GroupId = new Guid("1e0f6683-bfc7-4fe4-bafd-b7c52f497aa2");
        private static readonly String GroupName = "HarshPoint";

        private static readonly Guid TermSetId = new Guid("a5d59d30-10e8-4221-bf59-75a1d76f0be0");

        public TaxonomyResolving(SharePointClientFixture fixture, ITestOutputHelper output) : base(fixture, output)
        {
        }

        [Fact]
        public async Task Default_SiteCollection_TermStore_gets_resolved()
        {
            var resolver = ManualResolver.ResolveSingle(Resolve.TermStoreSiteCollectionDefault());
            await ClientContext.ExecuteQueryAsync();

            var termStore = resolver.Value;
            Assert.NotNull(termStore);

            var expected = TaxonomySession.GetDefaultSiteCollectionTermStore();

            Fixture.ClientContext.Load(expected, ts => ts.Id);
            Fixture.ClientContext.Load(termStore, ts => ts.Id);
            await Fixture.ClientContext.ExecuteQueryAsync();

            Assert.Equal(expected.Id, termStore.Id);
        }

        [Fact]
        public async Task Default_Keywords_TermStore_gets_resolved()
        {
            var resolver = ManualResolver.ResolveSingle(Resolve.TermStoreKeywordsDefault());
            await ClientContext.ExecuteQueryAsync();

            var termStore = resolver.Value;
            Assert.NotNull(termStore);

            var expected = TaxonomySession.GetDefaultKeywordsTermStore();

            Fixture.ClientContext.Load(expected, ts => ts.Id);
            Fixture.ClientContext.Load(termStore, ts => ts.Id);
            await Fixture.ClientContext.ExecuteQueryAsync();

            Assert.Equal(expected.Id, termStore.Id);
        }

        [Fact]
        public async Task TermSet_resolved_by_id()
        {
            var expected = await EnsureTestTermSet();

            var resolver = ManualResolver.ResolveSingle(
                Resolve.TermStoreSiteCollectionDefault().TermSet().ById(TermSetId),
                ts => ts.Id,
                ts => ts.Name
            );
            await ClientContext.ExecuteQueryAsync();

            var actual = resolver.Value;

            Assert.Equal(TermSetId, actual.Id);
            Assert.Equal(TermSetId.ToString("n"), actual.Name);
        }

        private async Task<TermSet> EnsureTestTermSet()
        {
            var store = TaxonomySession.GetDefaultSiteCollectionTermStore();
            var groups = Fixture.ClientContext.LoadQuery(store.Groups);

            await Fixture.ClientContext.ExecuteQueryAsync();

            var group = groups.FirstOrDefaultByProperty(x => x.Name, GroupName, StringComparer.Ordinal);

            if (group == null)
            {
                group = store.CreateGroup(GroupName, Guid.NewGuid());
                await Fixture.ClientContext.ExecuteQueryAsync();
            }

            var termSet = group.TermSets.GetById(TermSetId);

            try
            {
                await Fixture.ClientContext.ExecuteQueryAsync();
            }
            catch (ServerException)
            {
                termSet = group.CreateTermSet(TermSetId.ToString("n"), TermSetId, 1033);
                await Fixture.ClientContext.ExecuteQueryAsync();
            }

            return termSet;
        }
    }
}
