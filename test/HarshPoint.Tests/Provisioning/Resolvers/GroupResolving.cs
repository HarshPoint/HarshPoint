using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning.Resolvers
{
    public class GroupResolving : ResolverTestBase
    {
        public GroupResolving(ITestOutputHelper output)
            : base(output)
        {
        }

        [FactNeedsSharePoint]
        public async Task Group_gets_resolved_by_id()
        {
            var group = await CreateGroup(g => g.Title);

            var results = ManualResolver.Resolve(
                Resolve.Group().ById(group.Id),
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
        public async Task Group_gets_resolved_by_login_name()
        {
            var group = await CreateGroup(g => g.Title);

            var results = ManualResolver.Resolve(
                Resolve.Group().ByLoginName(group.LoginName),
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
