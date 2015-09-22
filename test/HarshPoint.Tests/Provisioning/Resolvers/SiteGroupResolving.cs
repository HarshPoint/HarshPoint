using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning.Resolvers
{
    public class SiteGroupResolving : ResolverTestBase
    {
        public SiteGroupResolving(ITestOutputHelper output)
            : base(output)
        {
        }

        [FactNeedsSharePoint]
        public async Task SiteGroup_gets_resolved_by_id()
        {
            var group = await CreateSiteGroup(g => g.Title);

            var results = ManualResolver.Resolve(
                Resolve.SiteGroup().ById(group.Id),
                g => g.Title
            );

            await ClientContext.ExecuteQueryAsync();

            var resolvedGroup = Assert.Single(results);

            Assert.NotNull(resolvedGroup);
            Assert.Equal(
                group.Title,
                resolvedGroup.Title,
                StringComparer.OrdinalIgnoreCase
            );
        }

        [FactNeedsSharePoint]
        public async Task SiteGroup_gets_resolved_by_name()
        {
            var group = await CreateSiteGroup(g => g.Title);

            var results = ManualResolver.Resolve(
                Resolve.SiteGroup().ByName(group.Title),
                g => g.Title
            );

            await ClientContext.ExecuteQueryAsync();

            var resolvedGroup = Assert.Single(results);

            Assert.NotNull(resolvedGroup);
            Assert.Equal(
                group.Title,
                resolvedGroup.Title,
                StringComparer.OrdinalIgnoreCase
            );
        }
    }
}
